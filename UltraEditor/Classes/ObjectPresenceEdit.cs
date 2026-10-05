namespace UltraEditor.Classes;

using System.Collections.Generic;
using UnityEngine;

/// <summary>Marks inactive objects retained only for undo; exclude descendants from level scans.</summary>
public sealed class HistoryStorage : MonoBehaviour
{
    public static bool Contains(Component item) => item && item.GetComponentInParent<HistoryStorage>(true) != null;
}

/// <summary>Retains the same object instance so references and previous transform edits survive deletion.</summary>
public sealed class ObjectPresenceEdit : IEditAction, IDiscardableEdit
{
    static readonly Dictionary<GameObject, int> references = new();
    readonly GameObject target;
    readonly Transform storage;
    readonly Transform parent;
    readonly int sibling;
    readonly bool active;
    readonly bool created;
    readonly TransformState state;
    bool discarded;

    public ObjectPresenceEdit(GameObject target, Transform storage, bool created)
    {
        this.target = target;
        this.storage = storage;
        this.created = created;
        parent = target.transform.parent;
        sibling = target.transform.GetSiblingIndex();
        active = target.activeSelf;
        state = new TransformState(target.transform);
        references.TryGetValue(target, out int count);
        references[target] = count + 1;
    }

    public bool Undo() => SetPresent(!created);
    public bool Redo() => SetPresent(created);

    public bool SetPresent(bool present)
    {
        if (!target || !storage || (parent != null && HistoryStorage.Contains(parent))) return false;
        bool stored = target.transform.parent == storage;
        if (present)
        {
            if (!stored) return false;
            // A destroyed parent cannot be restored as a root without changing the level's structure.
            if (!ReferenceEquals(parent, null) && !parent) return false;
            target.transform.SetParent(parent, false);
            state.Apply(target.transform);
            target.transform.SetSiblingIndex(sibling);
            target.SetActive(active);
        }
        else
        {
            if (stored || target.transform.parent != parent) return false;
            target.SetActive(false);
            target.transform.SetParent(storage, false);
        }
        return true;
    }

    public void Discard()
    {
        if (discarded) return;
        discarded = true;
        if (!references.TryGetValue(target, out int count)) return;
        if (count > 1) { references[target] = count - 1; return; }
        references.Remove(target);
        if (target && storage && target.transform.parent == storage) Object.Destroy(target);
    }
}
