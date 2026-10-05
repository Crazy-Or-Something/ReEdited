namespace UltraEditor.Classes;

using System.Collections.Generic;

public interface IEditAction
{
    bool Undo();
    bool Redo();
}

public interface IDiscardableEdit { void Discard(); }

/// <summary>A bounded session history. Invalid actions are skipped.</summary>
public sealed class EditHistory
{
    const int Capacity = 100;
    readonly List<IEditAction> undo = new();
    readonly List<IEditAction> redo = new();
    public bool CanUndo => undo.Count > 0;
    public bool CanRedo => redo.Count > 0;

    public void Clear()
    {
        DiscardAll(undo);
        DiscardAll(redo);
    }

    static void DiscardAll(List<IEditAction> actions)
    {
        foreach (var action in actions) (action as IDiscardableEdit)?.Discard();
        actions.Clear();
    }

    public void Record(IEditAction action)
    {
        undo.Add(action);
        if (undo.Count > Capacity)
        {
            (undo[0] as IDiscardableEdit)?.Discard();
            undo.RemoveAt(0);
        }
        DiscardAll(redo);
    }

    public bool Undo() => Apply(undo, redo, false);
    public bool Redo() => Apply(redo, undo, true);

    static bool Apply(List<IEditAction> source, List<IEditAction> destination, bool forward)
    {
        while (source.Count > 0)
        {
            var action = source[source.Count - 1];
            source.RemoveAt(source.Count - 1);
            if (!(forward ? action.Redo() : action.Undo()))
            {
                (action as IDiscardableEdit)?.Discard();
                continue;
            }
            destination.Add(action);
            return true;
        }
        return false;
    }
}
