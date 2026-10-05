namespace UltraEditor.Classes;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed = 30f;
    public float mouseSensitivity = 2f;
    public float shiftMultiplier = 3f;
    (int x, int y) savedMousePos = new(0, 0);

    public Light unlitLight = null;
    bool rightCandidate, looking;
    float rightStarted;
    Vector2 rightMotion;
    const float LookHoldSeconds = 0.2f;
    const float LookMotionThreshold = 2f;

    readonly List<RaycastResult> pointerHits = new();

    bool PointerOverControl()
    {
        if (EventSystem.current == null) return false;
        pointerHits.Clear();
        var pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        EventSystem.current.RaycastAll(pointer, pointerHits);
        foreach (var hit in pointerHits)
        {
            if (hit.module is not GraphicRaycaster || hit.gameObject == null) continue;
            // Full-screen decorative graphics must not block viewport camera input.
            if (hit.gameObject.GetComponentInParent<Selectable>() != null
                || hit.gameObject.GetComponentInParent<ScrollRect>() != null
                || hit.gameObject.GetComponentInParent<TMP_InputField>() != null) return true;
        }
        return false;
    }

    public bool moving()
    {
        return looking && Input.GetMouseButton(1) && Plugin.canMove() && !EditorManager.Instance.blocker.activeSelf;
    }

    public void Awake()
    {
        GameObject obj = new GameObject("CameraLight");
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        unlitLight = obj.AddComponent<Light>();

        unlitLight.range = 500;
        unlitLight.renderMode = LightRenderMode.ForcePixel;
        unlitLight.type = LightType.Point;

        unlitLight.enabled = false;
    }

    public void Update()
    {
        if (!Plugin.canMove() || EditorManager.Instance.blocker.activeSelf)
        {
            if (looking) Cursor.visible = true;
            rightCandidate = looking = false;
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            rightCandidate = !PointerOverControl();
            looking = false;
            rightStarted = Time.unscaledTime;
            rightMotion = Vector2.zero;
        }
        if (rightCandidate && Input.GetMouseButton(1))
        {
            rightMotion += new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            if (!looking && (Time.unscaledTime - rightStarted >= LookHoldSeconds || rightMotion.magnitude >= LookMotionThreshold))
            {
                looking = true;
                savedMousePos = MouseController.GetMousePos();
                Cursor.visible = false;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            bool showMenu = rightCandidate && !looking;
            rightCandidate = looking = false;
            Cursor.visible = true;
            if (showMenu) { EditorContextMenu.Show(Input.mousePosition); return; }
        }

        float speed = EditorSettings.MovementSpeed * (Input.GetKey(Plugin.shiftKey) ? EditorSettings.FastMovementMultiplier : 1f) * Mathf.Min(Time.unscaledDeltaTime, 0.1f);
        float horizontal = Input.GetAxisRaw("Horizontal") * speed;
        float vertical = Input.GetAxisRaw("Vertical") * speed;
        float ascend = (Input.GetKey(KeyCode.E) ? 1 : 0 - (Input.GetKey(KeyCode.Q) ? 1 : 0)) * speed;
        transform.Translate(new Vector3(horizontal, ascend, vertical));
        if (looking && Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * EditorSettings.LookMultiplier * (EditorManager.sensitivity / 50f);
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * EditorSettings.LookMultiplier * (EditorManager.sensitivity / 50f);
            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.Rotate(Vector3.right, -mouseY, Space.Self);
            MouseController.SetCursorPos(savedMousePos.x, savedMousePos.y);
        }
    }

    public void setUnlit(bool unlit)
    {
        unlitLight.enabled = unlit;
    }
}
