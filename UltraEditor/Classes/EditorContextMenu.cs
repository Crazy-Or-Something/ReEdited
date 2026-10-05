namespace UltraEditor.Classes;

using System;
using UnityEngine;

/// <summary>Viewport actions shown by a quick right click.</summary>
public sealed class EditorContextMenu : MonoBehaviour
{
    static EditorContextMenu instance;
    public static bool Visible => instance != null && instance.open;
    bool open, creating;
    Rect bounds;
    GUIStyle panel, row, title;
    Texture2D background, hover;

    void Awake() => instance = this;
    void OnDisable() => open = false;
    void OnDestroy()
    {
        if (instance == this) instance = null;
        if (background) Destroy(background);
        if (hover) Destroy(hover);
    }

    public static void Show(Vector2 position)
    {
        var manager = EditorManager.Instance;
        if (!instance || !manager || !manager.editorOpen || !manager.editorCanvas.activeInHierarchy
            || manager.blocker.activeSelf || manager.cameraSelector.dragging || EditorSettings.IsConfigOpen) return;
        var ray = manager.editorCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out var hit, 1000f, manager.editorCamera.cullingMask))
        {
            var target = hit.transform;
            while (target && !manager.IsObjectEditable(target.gameObject)) target = target.parent;
            if (target) manager.cameraSelector.SelectObject(target.gameObject);
        }
        instance.creating = false;
        instance.open = true;
        instance.bounds = new Rect(position.x, Screen.height - position.y, 250, 300);
        Cursor.visible = true;
    }

    void Update()
    {
        var manager = EditorManager.Instance;
        if (open && (!manager || !manager.editorOpen || !manager.editorCanvas.activeInHierarchy
            || manager.blocker.activeSelf || EditorSettings.IsConfigOpen || Input.GetKeyDown(KeyCode.Escape))) open = false;
    }

    static Texture2D Solid(Color color)
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    void InitializeStyles()
    {
        if (panel != null) return;
        background = Solid(new Color(0.16f, 0.16f, 0.17f));
        hover = Solid(new Color(0.28f, 0.28f, 0.3f));
        panel = new GUIStyle(GUI.skin.box) { padding = new RectOffset(6, 6, 6, 6) };
        panel.normal.background = background;
        row = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft, fontSize = 14,
            padding = new RectOffset(12, 8, 0, 0), border = new RectOffset(), margin = new RectOffset(0, 0, 1, 1) };
        row.normal.background = background;
        row.normal.textColor = Color.white;
        row.hover.background = hover;
        row.hover.textColor = Color.white;
        row.active.background = hover;
        title = new GUIStyle(GUI.skin.label) { fontSize = 12, padding = new RectOffset(12, 0, 3, 3) };
        title.normal.textColor = new Color(0.7f, 0.7f, 0.72f);
    }

    void OnGUI()
    {
        if (!open) return;
        InitializeStyles();
        GUI.depth = -1000;
        bounds.height = creating ? 186 : 354;
        bounds.x = Mathf.Clamp(bounds.x, 0, Mathf.Max(0, Screen.width - bounds.width));
        bounds.y = Mathf.Clamp(bounds.y, 0, Mathf.Max(0, Screen.height - bounds.height));
        var current = Event.current;
        if (current.type == EventType.MouseDown && !bounds.Contains(current.mousePosition))
        {
            open = false;
            current.Use();
            return;
        }
        var manager = EditorManager.Instance;
        var selector = manager.cameraSelector;
        bool editable = selector.selectedObject && manager.IsObjectEditable();
        GUILayout.BeginArea(bounds, panel);
        GUILayout.Label(creating ? "Create object" : "ReEdited", title);
        if (creating)
        {
            Row("< Back", () => creating = false, true, false);
            Row("Cube", () => manager.createCube());
            Row("Floor", () => manager.createFloor(new Vector3(10, 1, 10)));
            Row("Wall", () => manager.createFloor(new Vector3(10, 5, 1)));
        }
        else
        {
            Row("Create                         >", () => creating = true, true, false);
            Row("Copy", selector.CopySelected, editable);
            Row("Paste", selector.PasteClipboard, selector.CanPaste);
            Row("Duplicate", manager.duplicateObject, editable);
            Row("Delete", manager.deleteObject, editable);
            Row("Undo", selector.UndoEdit, selector.CanUndo);
            Row("Redo", selector.RedoEdit, selector.CanRedo);
            Row("Move", () => selector.selectionMode = CameraSelector.SelectionMode.Move, editable);
            Row("Rotate", () => selector.selectionMode = CameraSelector.SelectionMode.Rotate, editable);
            Row("Scale", () => selector.selectionMode = CameraSelector.SelectionMode.Scale, editable);
            Row("Focus selected", selector.FocusOnSelected, selector.selectedObject != null);
        }
        GUILayout.EndArea();
        if (bounds.Contains(current.mousePosition) && current.isMouse) current.Use();
    }

    void Row(string label, Action action, bool enabled = true, bool close = true)
    {
        GUI.enabled = enabled;
        if (GUILayout.Button(label, row, GUILayout.Height(25)))
        {
            if (close) open = false;
            action();
        }
        GUI.enabled = true;
    }
}
