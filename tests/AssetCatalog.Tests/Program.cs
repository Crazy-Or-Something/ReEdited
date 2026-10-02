using System;
using System.Linq;
using UltraEditor.Classes;

string zombie = "Assets/Prefabs/Enemies/Zombie.prefab";
string drone = "Assets/Prefabs/Enemies/Variants/Drone.prefab";
string door = "Assets/Prefabs/Levels/Doors/GardenDoor.prefab";
string[] discovered = [zombie, drone, zombie, door, "Assets/Prefabs/EnemiesExtra/NotAnEnemy.prefab"];
string[] recommended = [zombie, "AltarBlueOff", zombie, "BlackholeChaos/Whitehole", "Bonus"];
var folders = AssetCatalog.Create(discovered, recommended);
Assert(folders[AssetCatalog.Root].Count == 0, "The landing page contains folders, not a duplicate item list.");
Assert(folders[AssetCatalog.Root + "Enemies/"].SequenceEqual(new[] { drone, zombie }),
    "Enemies include nested folders, exclude similar prefixes, sort by display name, and remove duplicates.");
Assert(folders[AssetCatalog.Root + "Doors/"].Single() == door, "Door spawn keys remain unchanged.");
Assert(folders[AssetCatalog.Root + "Interactive/"].Single() == "AltarBlueOff", "Special aliases remain usable.");
Assert(folders[AssetCatalog.Root + "Obstacles/"].Single() == "BlackholeChaos/Whitehole", "Custom hazards are categorized.");
Assert(folders[AssetCatalog.Root + "Recommended/"].Contains("Bonus"), "Uncategorized recommendations remain accessible.");
Assert(!folders.ContainsKey(AssetCatalog.Root + "Effects/"), "Empty categories are hidden.");
Assert(AssetCatalog.Create([], ["", " "]).Count == 2, "Empty catalogs still have a root and recommendations.");
Assert(discovered.Length == 5 && recommended.Length == 5, "Source arrays are not changed.");
Console.WriteLine("All asset catalog checks passed.");
Assert(AssetCatalog.Search(discovered.Concat(recommended), "  ZOMBIE  ").SequenceEqual(new[] { zombie }),
    "Search ignores case and surrounding whitespace and removes duplicate keys.");
Assert(AssetCatalog.Search(discovered, "drone").Single() == drone, "Search finds assets in nested folders.");
Assert(AssetCatalog.Search(discovered, "Enemies").Count == 0, "Search matches names rather than directory names.");
Assert(AssetCatalog.Search(recommended, "altar").Single() == "AltarBlueOff", "Search preserves special spawn aliases.");
Assert(AssetCatalog.Search(discovered, "missing").Count == 0, "Missing names produce no results.");
Assert(AssetCatalog.Search(discovered, " ").Count == 0, "Blank input leaves search mode.");
Console.WriteLine("All asset search checks passed.");
Assert(AssetCatalog.GetFavorites(discovered, [zombie, zombie, "missing"]).SequenceEqual(new[] { zombie }),
    "Favorites exclude unavailable keys and remove duplicates without changing spawn keys.");
Assert(AssetCatalog.GetFavorites(discovered, [zombie, drone]).SequenceEqual(new[] { drone, zombie }),
    "Favorites are sorted by display name.");
Assert(AssetCatalog.GetFavorites(recommended, ["AltarBlueOff"]).Single() == "AltarBlueOff",
    "Custom aliases can be favorites.");
Assert(AssetCatalog.GetFavorites(discovered, []).Count == 0, "An empty favorites list produces no assets.");
Console.WriteLine("All asset favorites checks passed.");

static void Assert(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
