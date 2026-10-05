using System;
using UltraEditor.Classes;

Check(GridSnap.Snap(1.13f, 0.25f) == 1.25f, "Positive coordinates align to a fractional grid.");
Check(GridSnap.Snap(-1.13f, 0.25f) == -1.25f, "Negative coordinates align symmetrically.");
Check(GridSnap.Snap(0.11f, 0.25f) == 0f, "Small local drags remain at their starting position.");
Check(GridSnap.Snap(-0.26f, 0.25f) == -0.25f, "Local drag distances preserve their direction.");
Check(GridSnap.Snap(1.25f, 0.25f) == 1.25f, "Already aligned coordinates stay unchanged.");
Check(GridSnap.Snap(1.5f, 1f) == 2f && GridSnap.Snap(-1.5f, 1f) == -2f,
    "Midpoints retain the inherited nearest-even rounding behavior.");
Check(Math.Abs(GridSnap.Snap(0.137f, 0.01f) - 0.14f) < 0.00001f, "Small grid sizes remain usable.");
foreach (float invalid in new[] { 0f, -1f, float.NaN, float.PositiveInfinity })
    Check(GridSnap.Snap(1.13f, invalid) == 1.13f, "Invalid grid sizes do not corrupt positions.");
Check(GridSnap.Snap(float.MaxValue, 0.01f) == float.MaxValue, "Large coordinates do not overflow.");
Console.WriteLine("All grid snapping checks passed.");

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
