namespace UltraEditor.Classes;

using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public static class BundlesManager
{
    public static AssetBundle editorBundle;

    public static GameObject editorCanvas, levelCanvas, exploreLevelsCanvas, welcomeCanvas;

    public static Shader ghostDottedOutline;

    public static GameObject cloudPrefab, pyramidMesh, duvizPlushPrefab, duvizPlushFixedPrefab;

    public static void Load()
    {
        Stream bundleStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UltraEditor.Assets.editorcanvas.bundle");

        AssetBundleCreateRequest request = AssetBundle.LoadFromStreamAsync(bundleStream);
        request.completed += (op) => 
        {
            editorBundle = request.assetBundle;
            if (editorBundle == null)
            {
                Plugin.LogError("Failed to load AssetBundle from memory!");
                return;
            }

            LoadAssets();
            Plugin.LogInfo("Loaded embedded AssetBundle!");
        };
    }

    public static void LoadAssets()
    {
        editorCanvas = editorBundle.LoadAsset<GameObject>("Assets/Prefabs/EditorCanvas.prefab");
        exploreLevelsCanvas = editorBundle.LoadAsset<GameObject>("ExploreLevelsCanvas");
        welcomeCanvas = editorBundle.LoadAsset<GameObject>("WelcomeCanvas");
        levelCanvas = editorBundle.LoadAsset<GameObject>("OpenLevelCanvas");
        UpdateBranding(editorCanvas);
        UpdateBranding(exploreLevelsCanvas);
        UpdateBranding(welcomeCanvas);
        UpdateBranding(levelCanvas);

        ghostDottedOutline = editorBundle.LoadAsset<Shader>("GhostDottedOutline");

        cloudPrefab = editorBundle.LoadAsset<GameObject>("Cloud");
        pyramidMesh = editorBundle.LoadAsset<GameObject>("PyramidMesh");
        duvizPlushPrefab = editorBundle.LoadAsset<GameObject>("DuvizPlush");
        duvizPlushFixedPrefab = editorBundle.LoadAsset<GameObject>("DuvizPlushFixed");
    }

    // The inherited UI is embedded in a compiled bundle; update its title labels before instantiation.
    static void UpdateBranding(GameObject canvas)
    {
        if (!canvas) return;
        foreach (var label in canvas.GetComponentsInChildren<TMP_Text>(true)) label.text = BrandedTitle(label.text);
        foreach (var label in canvas.GetComponentsInChildren<Text>(true)) label.text = BrandedTitle(label.text);
    }

    static string BrandedTitle(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        var match = Regex.Match(text, @"^\s*(UltraEditor|UltraEdited)(\s+v\d+(\.\d+)*.*?)?\s*$", RegexOptions.IgnoreCase);
        if (!match.Success) return text;
        return match.Groups[2].Success ? $"{Plugin.Name} v{Plugin.Version}" : Plugin.Name;
    }
}
