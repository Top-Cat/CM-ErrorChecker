using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI
{
    private GameObject popup;
    private TMP_Dropdown dropdownComponent;

    private readonly ErrorChecker plugin;
    private readonly List<Check> checks;

    public TextMeshProUGUI problemInfoText;

    private GameObject paramContainer;
    public readonly List<UITextInput> paramTexts = new List<UITextInput>();
    private readonly List<UIButton> navigation = new List<UIButton>();

    private readonly ExtensionButton errorCheckerButton = new ExtensionButton();

    public UI(ErrorChecker plugin, List<Check> checks)
    {
        this.plugin = plugin;
        this.checks = checks;

        errorCheckerButton.Tooltip = "Check Errors";

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ErrorChecker.Icon.png"))
        {
            var len = (int) stream.Length;
            var bytes = new byte[len];
            stream.Read(bytes, 0, len);

            var texture2D = new Texture2D(512, 512);
            texture2D.LoadImage(bytes);

            errorCheckerButton.Icon = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0), 100.0f, 0, SpriteMeshType.Tight);
        }

        ExtensionButtons.AddButton(errorCheckerButton);
    }

    public void AddButton(MapEditorUI rootObj)
    {
        AddPopup(rootObj);
        popup.SetActive(false);

        errorCheckerButton.OnClick = () =>
        {
            popup.SetActive(!popup.activeSelf);
        };
    }

    private UIButton GenerateButton(Transform parent, string title, string text, Vector2 pos, UnityAction onClick, Vector2? size = null)
    {
        var button = Object.Instantiate(PersistentUI.Instance.ButtonPrefab, parent);
        MoveTo(button.transform, size?.x ?? 70, size?.y ?? 25, 0.5f, 1, pos.x, pos.y);

        button.name = title;
        button.Button.onClick.AddListener(onClick);

        button.SetText(text);
        button.Text.enableAutoSizing = false;
        button.Text.fontSize = 12;

        return button;
    }

    private void GenerateButton(Transform parent, string title, Sprite buttonImage, Vector2 pos, UnityAction onClick, Vector2? size = null)
    {
        var button = Object.Instantiate(PersistentUI.Instance.ButtonPrefab, parent);
        MoveTo(button.transform, size?.x ?? 70, size?.y ?? 25, 0.5f, 1, pos.x, pos.y);

        button.name = title;
        button.Button.onClick.AddListener(onClick);
        button.SetImage(buttonImage);
    }

    private void AddEntry(string title, float y, string def)
    {
        var parent = paramContainer.transform;

        var entryLabel = new GameObject(title + " Label", typeof(TextMeshProUGUI));
        var rectTransform = ((RectTransform) entryLabel.transform);
        rectTransform.SetParent(parent);

        MoveTo(rectTransform, 75, 17, 0.5f, 1, -52.5f, y);
        var textComponent = entryLabel.GetComponent<TextMeshProUGUI>();

        textComponent.font = PersistentUI.Instance.ButtonPrefab.Text.font;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.enableAutoSizing = true;
        textComponent.fontSizeMin = 8;
        textComponent.fontSizeMax = 16;
        textComponent.text = title;

        var textInput = Object.Instantiate(PersistentUI.Instance.TextInputPrefab, parent);
        MoveTo(textInput.Transform, 80, 20, 0.5f, 1, 30, y);
        textInput.GetComponent<Image>().pixelsPerUnitMultiplier = 3;
        textInput.InputField.text = def;
        textInput.InputField.onFocusSelectAll = false;
        textInput.InputField.textComponent.alignment = TextAlignmentOptions.Left;
        textInput.InputField.textComponent.fontSize = 12;

        paramTexts.Add(textInput);
    }

    public void AddPopup(MapEditorUI rootObj)
    {
        var parent = rootObj.mainUIGroup[5];

        popup = new GameObject("ErrorChecker Popup");
        popup.transform.parent = parent.transform;

        AttachTransform(popup, 220, 151, 0.5f, 1, -434, -5, 0.5f, 1);
        var image = popup.AddComponent<Image>();

        image.sprite = PersistentUI.Instance.Sprites.Background;
        image.type = Image.Type.Sliced;
        image.color = new Color(0.24f, 0.24f, 0.24f, 1);

        AddDropdown(popup);

        GenerateButton(popup.transform, "Reload", PersistentUI.Instance.Sprites.Reload, new Vector2(95, -23), () => {
            checks[dropdownComponent.value].Reload();
            UpdateSelected(dropdownComponent.value);
        }, new Vector2(22, 25));

        ////////

        paramContainer = new GameObject("Param Container");
        paramContainer.transform.parent = popup.transform;
        AttachTransform(paramContainer, 200, 151, 0.5f, 0.5f, 0, 0);

        AddEntry("Min Time", -54, "0.24");
        AddEntry("Max Time", -77, "0.75");

        ////////

        navigation.Clear();

        navigation.Add(GenerateButton(popup.transform, "Perform", "Run", new Vector2(0, -105), () => {
            plugin.CheckErrors(checks[dropdownComponent.value]);
        }));

        navigation.Add(GenerateButton(popup.transform, "Previous", "<", new Vector2(-50, -105), () => {
            plugin.NextBlock(-1);
        }, new Vector2(22, 25)));

        navigation.Add(GenerateButton(popup.transform, "Next", ">", new Vector2(50, -105), () => {
            plugin.NextBlock(1);
        }, new Vector2(22, 25)));

        ////////

        GameObject problemInfo = new GameObject("Problem Info");
        problemInfo.transform.parent = popup.transform;

        var transform3 = AttachTransform(problemInfo, 125, 17, 0.5f, 1, 0, -122, 0.5f, 1);
        problemInfoText = problemInfo.AddComponent<TextMeshProUGUI>();

        problemInfoText.font = PersistentUI.Instance.ButtonPrefab.Text.font;
        problemInfoText.alignment = TextAlignmentOptions.Top;

        problemInfoText.text = "...";
        problemInfoText.fontSize = 12;
        problemInfoText.enableWordWrapping = true;
        transform3.sizeDelta = new Vector2(190, 50);
    }

    private void UpdateSelected(int i)
    {
        paramContainer.SetActive(false);
        foreach (Transform child in paramContainer.transform)
        {
            Object.Destroy(child.gameObject);
        }
        paramTexts.Clear();

        checks[i].OnSelected();
        var vals = checks[i].Params;
        float y = -54;
        foreach (var v in vals)
        {
            AddEntry(v.name, y, v.def.ToString());
            y -= 23;
        }

        paramContainer.GetComponent<RectTransform>().sizeDelta = popup.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 51 - y);
        foreach (var rt2 in navigation)
        {
            rt2.Transform.anchoredPosition = new Vector3(rt2.Transform.anchoredPosition.x, y - 5, 0);
        }
        problemInfoText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, y - 22, 0);

        // Generates the caret in the text fields, who knows
        paramContainer.SetActive(true);
    }

    public void AddDropdown(GameObject parent)
    {
        var dropdown = Object.Instantiate(PersistentUI.Instance.DropdownPrefab, parent.transform);
        MoveTo(dropdown.transform, 186, 30, 0.5f, 1, -10, -23);
        dropdown.SetOptions(checks.Select(it => it.Name).ToList());
        dropdown.Dropdown.onValueChanged.AddListener(UpdateSelected);

        dropdownComponent = dropdown.Dropdown;
    }

    private void MoveTo(Transform transform, float width, float height, float anchorX, float anchorY, float x, float y, float p1 = 0.5f, float p2 = 0.5f)
    {
        if (!(transform is RectTransform rectTransform)) return;

        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.pivot = new Vector2(p1, p2);
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(anchorX, anchorY);
        rectTransform.anchoredPosition = new Vector3(x, y, 0);
    }

    private RectTransform AttachTransform(GameObject obj, float width, float height, float anchorX, float anchorY, float x, float y, float p1 = 0.5f, float p2 = 0.5f)
    {
        var rectTransform = obj.AddComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.pivot = new Vector2(p1, p2);
        rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(anchorX, anchorY);
        rectTransform.anchoredPosition = new Vector3(x, y, 0);

        return rectTransform;
    }
}
