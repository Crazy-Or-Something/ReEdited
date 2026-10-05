namespace UltraEditor.Classes;

using System;

/// <summary>Rounds a coordinate or signed movement distance to the nearest grid step.</summary>
public static class GridSnap
{
    public static float Snap(float value, float step)
    {
        if (float.IsNaN(value) || float.IsInfinity(value)
            || float.IsNaN(step) || float.IsInfinity(step) || step <= 0f) return value;
        // Use double arithmetic so small valid steps do not overflow the division.
        return (float)(Math.Round((double)value / step, MidpointRounding.ToEven) * step);
    }
}
