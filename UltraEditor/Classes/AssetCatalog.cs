namespace UltraEditor.Classes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary> Virtual folders that retain the original keys used to spawn and save assets. </summary>
public static class AssetCatalog
{
    public const string Root = "Assets/ReEdited/";

    public static List<string> Search(IEnumerable<string> keys, string query)
    {
        query = query?.Trim() ?? "";
        if (query.Length == 0) return [];
        return Sort(keys.Where(key => !string.IsNullOrWhiteSpace(key) &&
            Path.GetFileNameWithoutExtension(key).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0));
    }

    public static Dictionary<string, List<string>> Create(
        IEnumerable<string> discoveredKeys, IEnumerable<string> recommendedKeys)
    {
        List<string> recommended = Sort(recommendedKeys);
        List<string> available = Sort(discoveredKeys.Concat(recommended));
        Dictionary<string, List<string>> folders = new(StringComparer.Ordinal)
        {
            [Root] = [],
            [Root + "Recommended/"] = recommended,
        };

        Add("Enemies", "Assets/Prefabs/Enemies/");
        Add("Decorations", "Assets/Prefabs/Levels/Decorations/", "ImCloudingIt", "DuvizPlushFixed");
        Add("Interactive", "Assets/Prefabs/Levels/Interactive/", "AltarBlueOff", "AltarRedOff");
        Add("Doors", "Assets/Prefabs/Levels/Doors/");
        Add("Obstacles", "Assets/Prefabs/Levels/Obstacles/", "BlackholeChaos/Blackhole", "BlackholeChaos/Whitehole");
        Add("Sandbox", "Assets/Prefabs/Sandbox/");
        Add("Effects", "Assets/Particles/");
        Add("Special Rooms", "Assets/Prefabs/Levels/Special Rooms/");
        return folders;

        void Add(string name, string prefix, params string[] aliases)
        {
            List<string> matches = available.Where(key =>
                key.StartsWith(prefix, StringComparison.Ordinal) || aliases.Contains(key)).ToList();
            if (matches.Count > 0)
                folders[Root + name + "/"] = matches;
        }
    }

    private static List<string> Sort(IEnumerable<string> keys) => keys
        .Where(key => !string.IsNullOrWhiteSpace(key))
        .Distinct(StringComparer.Ordinal)
        .OrderBy(key => Path.GetFileNameWithoutExtension(key), StringComparer.OrdinalIgnoreCase)
        .ThenBy(key => key, StringComparer.Ordinal)
        .ToList();
}
