namespace UltraEditor.Classes;

using UnityEngine;

/// <summary>Local transform values preserve placement under an unchanged parent.</summary>
public readonly struct TransformState
{
    readonly Vector3 position;
    readonly Quaternion rotation;
    readonly Vector3 scale;

    public TransformState(Transform target)
    {
        position = target.localPosition;
        rotation = target.localRotation;
        scale = target.localScale;
    }

    public bool Matches(TransformState other)
        => position.Equals(other.position) && rotation.Equals(other.rotation) && scale.Equals(other.scale);

    public void Apply(Transform target)
    {
        target.localPosition = position;
        target.localRotation = rotation;
        target.localScale = scale;
    }
}

public sealed class TransformEdit : IEditAction
{
    readonly Transform target;
    readonly Transform parent;
    readonly TransformState before;
    readonly TransformState after;

    public TransformEdit(Transform target, TransformState before, TransformState after)
    {
        this.target = target;
        parent = target.parent;
        this.before = before;
        this.after = after;
    }

    public bool Undo() => Apply(after, before);
    public bool Redo() => Apply(before, after);

    bool Apply(TransformState expected, TransformState state)
    {
        // Deleted, reparented, or externally edited objects must not receive stale transforms.
        if (!target || HistoryStorage.Contains(target) || target.parent != parent || !expected.Matches(new TransformState(target))) return false;
        state.Apply(target);
        return true;
    }
}
