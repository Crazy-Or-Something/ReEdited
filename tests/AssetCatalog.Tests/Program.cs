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

static void Assert(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
