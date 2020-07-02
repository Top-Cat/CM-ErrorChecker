using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ErrorChecker
{
    class UI
    {
        public static void AddButton(MapEditorUI rootObj, UnityAction CheckErrors)
        {
            var parent = rootObj.mainUIGroup[3];

            GameObject button = new GameObject();
            button.name = "ErrorCheckerButton";
            button.transform.parent = parent.transform;

            var rectTransform = button.AddComponent<RectTransform>();
            var image = button.AddComponent<Image>();
            var buttonObj = button.AddComponent<Button>();

            buttonObj.onClick.AddListener(CheckErrors);

            rectTransform.sizeDelta = new Vector2(70, 25);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.anchoredPosition = new Vector3(330, -20, 0);

            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            image.type = Image.Type.Sliced;
            image.color = new Color(0.4f, 0.4f, 0.4f, 1);

            GameObject text = new GameObject();
            text.name = "Text";
            text.transform.parent = button.transform;

            var rectTransform2 = text.AddComponent<RectTransform>();
            var textComponent = text.AddComponent<TextMeshProUGUI>();

            rectTransform2.localScale = new Vector3(1, 1, 1);
            rectTransform2.anchoredPosition = new Vector3(0, 0, 0);

            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.SetText("Check Errors");
            textComponent.fontSize = 12;
        }
    }
}
