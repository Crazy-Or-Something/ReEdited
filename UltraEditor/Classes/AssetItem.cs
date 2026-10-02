namespace UltraEditor.Classes;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AssetItem : MonoBehaviour, IPointerClickHandler
{
    public string assetPath;
    public string assetName;

    public TMP_Text assetNameText;
    public GameObject assetItemObject;

    public void Start()
    {
        if (string.IsNullOrEmpty(assetPath))
            assetPath = assetItemObject.name;
        if (assetNameText != null)
        {
            string label = string.IsNullOrEmpty(assetName) ? assetNameText.text : assetName;
            assetNameText.text = (AssetsWindowManager.IsFavorite(assetPath) ? "[*] " : "") + label;
        }
        
        GetComponent<Button>()?.onClick.AddListener(() =>
        {
            EditorManager.Instance.SpawnAsset(assetPath);
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            AssetsWindowManager.Instance?.ToggleFavorite(assetPath);
    }
}
