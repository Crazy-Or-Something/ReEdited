using System;
using UltraEditor.Classes;

var history = new EditHistory();
var value = new Value();
Check(!history.Undo() && !history.Redo(), "Empty history is harmless.");
Record(0, 1);
Record(1, 2);
Check(history.Undo() && value.Number == 1, "Undo applies the most recent action first.");
Check(history.Undo() && value.Number == 0, "Undo traverses multiple actions.");
Check(history.Redo() && value.Number == 1, "Redo restores actions in order.");
Record(1, 3);
Check(!history.Redo(), "A new edit discards the abandoned redo branch.");
Check(history.Undo() && value.Number == 1, "The new branch remains undoable.");
Check(history.Redo() && value.Number == 3, "The new branch remains redoable.");

var invalid = new Change(value, 3, 4) { Valid = false };
history.Record(invalid);
Check(history.Undo() && value.Number == 1, "Invalid actions are skipped to reach the next valid action.");
Check(history.Redo() && value.Number == 3, "Invalid actions never enter the redo stack.");

history = new EditHistory();
value.Number = 0;
for (int i = 0; i < 105; i++) Record(i, i + 1);
int undone = 0;
while (history.Undo()) undone++;
Check(undone == 100 && value.Number == 5, "Only the most recent 100 actions are retained.");
int redone = 0;
while (history.Redo()) redone++;
Check(redone == 100 && value.Number == 105, "The entire retained history can be redone.");
Console.WriteLine("All edit history checks passed.");

var cleanupHistory = new EditHistory();
var retained = new CleanupAction();
cleanupHistory.Record(retained);
Check(cleanupHistory.Undo(), "Retained objects can enter redo history.");
cleanupHistory.Record(new CleanupAction());
Check(retained.Discarded == 1, "Abandoning a redo branch releases its retained objects once.");
var expired = new CleanupAction();
cleanupHistory.Clear();
cleanupHistory.Record(expired);
for (int i = 0; i < 100; i++) cleanupHistory.Record(new CleanupAction());
Check(expired.Discarded == 1, "Evicting an old action releases its retained objects.");
var cleared = new CleanupAction();
cleanupHistory.Record(cleared);
cleanupHistory.Clear();
Check(cleared.Discarded == 1 && !cleanupHistory.CanUndo && !cleanupHistory.CanRedo,
    "Clearing a level releases history resources and disables its menu actions.");
var rejected = new CleanupAction { Valid = false };
cleanupHistory.Record(rejected);
Check(!cleanupHistory.Undo() && rejected.Discarded == 1, "Skipping an invalid action releases its resources.");
Console.WriteLine("All history resource cleanup checks passed.");

void Record(int before, int after)
{
    value.Number = after;
    history.Record(new Change(value, before, after));
}

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

sealed class Value { public int Number; }

sealed class CleanupAction : IEditAction, IDiscardableEdit
{
    public bool Valid = true;
    public int Discarded;
    public bool Undo() => Valid;
    public bool Redo() => Valid;
    public void Discard() => Discarded++;
}

sealed class Change(Value target, int before, int after) : IEditAction
{
    public bool Valid = true;
    public bool Undo()
    {
        if (!Valid) return false;
        target.Number = before;
        return true;
    }
    public bool Redo()
    {
        if (!Valid) return false;
        target.Number = after;
        return true;
    }
}
