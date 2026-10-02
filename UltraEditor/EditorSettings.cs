namespace UltraEditor;

using System.Collections.Generic;
using ThornClient.Core;
using ThornClient.Core.ConfigurableElements;
using ThornClient.Core.DataTypes;
using ThornClient.Managers;
using ThornClient.System;
using UltraEditor.Classes;
using UnityEngine;

/// <summary>Persistent editor preferences exposed through Thorn Core.</summary>
public static class EditorSettings
{
    public const string DependencyGuid = "com.github.end-4.thornClient";
    public const string MinimumVersion = "0.5.0";
    static ReEditedSettingsModule settings;

    public static bool IsConfigOpen => ClickGUI.Instance != null && ClickGUI.Instance.IsEnabled;
    public static float MovementSpeed => Bounded(settings?.MovementSpeed.Value ?? 30f, 30f, 1f, 200f);
    public static float LookMultiplier => Bounded(settings?.LookMultiplier.Value ?? 1f, 1f, 0.1f, 5f);
    public static float FastMovementMultiplier => Bounded(settings?.FastMovementMultiplier.Value ?? 3f, 3f, 1f, 10f);

    public static void Initialize()
    {
        if (settings != null) return;
        // Thorn discovers modules from loaded assemblies before dependent plugins awaken.
        settings = ModuleManager.Get<ReEditedSettingsModule>();
        if (settings != null) return;
        settings = new ReEditedSettingsModule();
        ModuleManager.Items.Add(settings);
        ConfigManager.LoadConfig(settings);
    }

    public static bool KeyDown(string id, KeyCode fallback) => IsPressed(id, fallback, false);
    public static bool KeyHeld(string id, KeyCode fallback) => IsPressed(id, fallback, true);

    static bool IsPressed(string id, KeyCode fallback, bool held)
    {
        if (IsConfigOpen || AssetsWindowManager.IsSearchFocused) return false;
        Keybind binding = null;
        if (settings != null && settings.Keys.TryGetValue(id, out var setting)) binding = setting.Value;
        var key = binding?.Key ?? fallback;
        var modifier = binding?.Modifier ?? KeyCode.None;
        return key != KeyCode.None && (modifier == KeyCode.None || Input.GetKey(modifier))
            && (held ? Input.GetKey(key) : Input.GetKeyDown(key));
    }

    static float Bounded(float value, float fallback, float min, float max)
        => float.IsNaN(value) || float.IsInfinity(value) ? fallback : Mathf.Clamp(value, min, max);
}

/// <summary>Thorn's automatically discovered configuration module for ReEdited.</summary>
public sealed class ReEditedSettingsModule : ThornClient.Core.Module
{
    public Dictionary<string, Setting<Keybind>> Keys { get; } = new();
    public Setting<float> MovementSpeed { get; }
    public Setting<float> LookMultiplier { get; }
    public Setting<float> FastMovementMultiplier { get; }

    public ReEditedSettingsModule() : base("mysticred.reedited.settings", "ReEdited",
        "Customize the level editor's controls and camera.", ModuleCategory.Utility, hasToggling: false)
    {
        var shortcuts = CreateGroup("shortcuts", "Keyboard shortcuts", "Keys used while editing levels.");
        BindKey(shortcuts, "toggle_editor", "Toggle editor", KeyCode.F1);
        BindKey(shortcuts, "select_tool", "Select tool", KeyCode.F2);
        BindKey(shortcuts, "move_tool", "Move tool", KeyCode.F3);
        BindKey(shortcuts, "scale_tool", "Scale tool", KeyCode.F4);
        BindKey(shortcuts, "rotate_tool", "Rotate tool", KeyCode.F5);
        BindKey(shortcuts, "toggle_ui", "Toggle editor UI", KeyCode.F9);
        BindKey(shortcuts, "delete_object", "Delete object", KeyCode.Delete);
        BindKey(shortcuts, "create_cube", "Create cube", KeyCode.KeypadPlus);

        var camera = CreateGroup("camera", "Editor camera", "Movement and mouse look while editing.");
        MovementSpeed = CreateSetting("movement_speed", "Movement speed", "Camera movement speed.", 30f, camera);
        MovementSpeed.Hints = InterfaceHints.RangeHint(1f, 200f);
        LookMultiplier = CreateSetting("look_multiplier", "Look sensitivity multiplier",
            "Multiplies the game's existing mouse sensitivity.", 1f, camera);
        LookMultiplier.Hints = InterfaceHints.RangeHint(0.1f, 5f);
        FastMovementMultiplier = CreateSetting("fast_movement_multiplier", "Fast movement multiplier",
            "Movement multiplier while holding Shift.", 3f, camera);
        FastMovementMultiplier.Hints = InterfaceHints.RangeHint(1f, 10f);
    }

    void BindKey(SettingGroup parent, string id, string label, KeyCode defaultKey)
        => Keys.Add(id, CreateSetting(id, label, "Choose a key and an optional modifier.", new Keybind(defaultKey), parent));
}
