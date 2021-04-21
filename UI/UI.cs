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

    private ErrorChecker plugin;
    private List<Check> checks;

    public TextMeshProUGUI problemInfoText;

    private GameObject paramContainer;
    public List<TMP_InputField> paramTexts = new List<TMP_InputField>();
    public List<RectTransform> navigation = new List<RectTransform>();

    private Sprite UISprite;
    private Sprite DropdownArrow;
    private Sprite Checkmark;
    private Sprite Background;
    public Sprite ReloadSprite;

    private TMP_FontAsset font;

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

        Debug.Log("add button");
        ExtensionButtons.AddButton(errorCheckerButton);
    }

    public void AddButton(MapEditorUI rootObj)
    {
        var parent = rootObj.mainUIGroup[5];

        // Please don't judge me, these should probably be bundled with this plugin but this works for now
        var existingSliderImages = rootObj.GetComponentInChildren<SongTimelineController>().GetComponentInChildren<Slider>().GetComponentsInChildren<Image>();
        foreach (var s in existingSliderImages)
        {
            if (s.gameObject.name == "Background")
            {
                Background = s.sprite;
            }
        }

        var existingDropdown = rootObj.GetComponentInChildren<TMP_Dropdown>(true);
        var text = existingDropdown.GetComponentInChildren<TextMeshProUGUI>(true);
        font = text.font;
        UISprite = existingDropdown.image.sprite;
        var sprites = existingDropdown.GetComponentsInChildren<Image>(true);
        foreach (var s in sprites)
        {
            if (s.gameObject.name == "Arrow")
            {
                DropdownArrow = s.sprite;
            }
            else if (s.gameObject.name == "Item Checkmark")
            {
                Checkmark = s.sprite;
            }
        }

        AddPopup(rootObj);
        popup.SetActive(false);

        //GenerateButton(parent.transform, "ErrorChecker Button", "Check Errors", new Vector2(-360, -20), () =>

        errorCheckerButton.OnClick = () =>
        {
            foreach (var rt in navigation)
            {
                var txt = rt.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.fontSize = 12;
            }
            popup.SetActive(!popup.activeSelf);
        };
    }

    private RectTransform GenerateButton(Transform parent, string title, string text, Vector2 pos, UnityAction onClick, Vector2? size = null)
    {
        var button = new GameObject();
        button.name = title;
        button.transform.parent = parent;

        var rt = AttachTransform(button, size?.x ?? 70, size?.y ?? 25, 0.5f, 1, pos.x, pos.y);
        var image = button.AddComponent<Image>();
        var buttonObj = button.AddComponent<Button>();
        button.AddComponent<Mask>();

        buttonObj.onClick.AddListener(onClick);

        image.sprite = UISprite;
        image.type = Image.Type.Sliced;
        image.color = new Color(0.4f, 0.4f, 0.4f, 1);

        var textObj = new GameObject();
        textObj.name = "Text";
        textObj.transform.parent = button.transform;

        AttachTransform(textObj, 200, 50, 0.5f, 0.5f, 0, 0);
        var textComponent = textObj.AddComponent<TextMeshProUGUI>();

        textComponent.font = font;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.SetText(text);
        textComponent.fontSize = 12;

        return rt;
    }

    private RectTransform GenerateButton(Transform parent, string title, Sprite buttonImage, Vector2 pos, UnityAction onClick, Vector2? size = null)
    {
        var button = new GameObject();
        button.name = title;
        button.transform.parent = parent;

        var rt = AttachTransform(button, size?.x ?? 70, size?.y ?? 25, 0.5f, 1, pos.x, pos.y);
        var image = button.AddComponent<Image>();
        var buttonObj = button.AddComponent<Button>();
        button.AddComponent<Mask>();

        buttonObj.onClick.AddListener(onClick);

        image.sprite = UISprite;
        image.type = Image.Type.Sliced;
        image.color = new Color(0.4f, 0.4f, 0.4f, 1);

        var textObj = new GameObject();
        textObj.name = "Text";
        textObj.transform.parent = button.transform;

        AttachTransform(textObj, 10, 12, 0.5f, 0.5f, 0, 0);
        var textComponent = textObj.AddComponent<Image>();

        textComponent.sprite = buttonImage;
        textComponent.type = Image.Type.Sliced;
        textComponent.color = Color.white;

        return rt;
    }

    private TMP_InputField AddEntry(string title, float y, string def)
    {
        Transform parent = paramContainer.transform;

        GameObject minTimeLabel = new GameObject();
        minTimeLabel.name = title + " Label";
        minTimeLabel.transform.parent = parent;

        var transform = AttachTransform(minTimeLabel, 75, 17, 0.5f, 1, -52.5f, y);
        var textComponent = minTimeLabel.AddComponent<TextMeshProUGUI>();
        transform.sizeDelta = new Vector2(75, 17); // TMP resets this because it hates me

        textComponent.font = font;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.enableAutoSizing = true;
        textComponent.fontSizeMin = 8;
        textComponent.fontSizeMax = 16;
        textComponent.text = title;

        GameObject minTimeText = new GameObject();
        minTimeText.name = title + " Text";
        minTimeText.transform.parent = parent;

        AttachTransform(minTimeText, 80, 20, 0.5f, 1, 30, y);
        var inputComponent = minTimeText.AddComponent<TMP_InputField>();
        var image2 = minTimeText.AddComponent<Image>();
        image2.sprite = UISprite;
        image2.type = Image.Type.Sliced;
        image2.pixelsPerUnitMultiplier = 3;
        image2.color = new Color(0.3f, 0.3f, 0.3f, 1);

        GameObject textArea = new GameObject();
        textArea.name = "Text Area";
        textArea.transform.parent = minTimeText.transform;

        var rt = textArea.AddComponent<RectTransform>();
        textArea.AddComponent<RectMask2D>();
        
        StretchTransform(rt);
        rt.offsetMin = new Vector2(5, 4);
        rt.offsetMax = new Vector2(-5, -5);
        inputComponent.textViewport = rt;

        GameObject minTimeTmp = new GameObject();
        minTimeTmp.name = "Text";
        minTimeTmp.transform.parent = textArea.transform;

        var rt2 = minTimeTmp.AddComponent<RectTransform>();
        StretchTransform(rt2);
        var txtComponent = minTimeTmp.AddComponent<IText>();
        txtComponent.font = font;
        inputComponent.textComponent = txtComponent;
        inputComponent.text = def;
        inputComponent.onFocusSelectAll = false;

        paramTexts.Add(inputComponent);
        return inputComponent;
    }

    public void AddPopup(MapEditorUI rootObj)
    {
        var parent = rootObj.mainUIGroup[5];

        popup = new GameObject();
        popup.name = "ErrorChecker Popup";
        popup.transform.parent = parent.transform;

        AttachTransform(popup, 220, 151, 0.5f, 1, -434, -5, 0.5f, 1);
        var image = popup.AddComponent<Image>();

        image.sprite = Background;
        image.type = Image.Type.Sliced;
        image.color = new Color(0.24f, 0.24f, 0.24f, 1);

        AddDropdown(popup);

        GenerateButton(popup.transform, "Reload", ReloadSprite, new Vector2(95, -23), () => {
            checks[dropdownComponent.value].Reload();
            UpdateSelected(dropdownComponent.value);
        }, new Vector2(22, 25));

        ////////

        paramContainer = new GameObject();
        paramContainer.name = "Param Container";
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

        GameObject problemInfo = new GameObject();
        problemInfo.name = "Problem Info";
        problemInfo.transform.parent = popup.transform;

        var transform3 = AttachTransform(problemInfo, 125, 17, 0.5f, 1, 0, -122, 0.5f, 1);
        problemInfoText = problemInfo.AddComponent<TextMeshProUGUI>();

        problemInfoText.font = font;
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
            rt2.anchoredPosition = new Vector3(rt2.anchoredPosition.x, y - 5, 0);
        }
        problemInfoText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, y - 22, 0);

        // Generates the caret in the text fields, who knows
        paramContainer.SetActive(true);
    }

    public void AddDropdown(GameObject parent)
    {
        GameObject dropdown = new GameObject();
        dropdown.name = "Check Type";
        dropdown.transform.parent = parent.transform;

        AttachTransform(dropdown, 186, 30, 0.5f, 1, -10, -23);
        dropdownComponent = dropdown.AddComponent<TMP_Dropdown>();
        dropdownComponent.AddOptions(checks.Select(it => it.Name).ToList());

        var image = dropdown.AddComponent<Image>();

        image.sprite = UISprite;
        image.type = Image.Type.Sliced;
        image.color = new Color(0.18f, 0.18f, 0.18f, 1);

        dropdownComponent.targetGraphic = image;
        dropdownComponent.onValueChanged.AddListener(UpdateSelected);

        ///////

        GameObject label = new GameObject();
        label.name = "Label";
        label.transform.parent = dropdown.transform;

        var transform = AttachTransform(label, 125, 17, 0.5f, 0.5f, -7.5f, -0.5f);
        var textComponent = label.AddComponent<TextMeshProUGUI>();
        transform.sizeDelta = new Vector2(125, 17); // TMP resets this because it hates me

        textComponent.font = font;
        textComponent.alignment = TextAlignmentOptions.Left;
        textComponent.fontSize = 14;

        dropdownComponent.captionText = textComponent;

        ///////

        GameObject arrow = new GameObject();
        arrow.name = "Arrow";
        arrow.transform.parent = dropdown.transform;

        AttachTransform(arrow, 20, 20, 1, 0.5f, -15, 0);
        var image2 = arrow.AddComponent<Image>();

        Font ArialFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        image2.sprite = DropdownArrow;
        image2.material = ArialFont.material;
        image2.color = Color.white;

        ////////

        GameObject template = new GameObject();
        template.name = "Template";
        template.SetActive(false);
        template.transform.parent = dropdown.transform;

        var rt = AttachTransform(template, 175, 20, 0.5f, 1, 0, -27.5f, 0.5f, 1);

        dropdownComponent.template = rt;

        ////////

        GameObject itemTemplate = new GameObject();
        itemTemplate.name = "Item";
        itemTemplate.transform.parent = template.transform;

        AttachTransform(itemTemplate, 175, 20, 0.5f, 0.5f, 0, 0);
        var toggle = itemTemplate.AddComponent<Toggle>();

        toggle.colors = new ColorBlock
        {
            normalColor = new Color(0.2f, 0.2f, 0.2f, 1),
            selectedColor = new Color(0.4f, 0.4f, 0.4f, 1),
            highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1),
            pressedColor = new Color(0.4f, 0.4f, 0.4f, 1),
            colorMultiplier = 1
        };

        ///////

        GameObject templateBg = new GameObject();
        templateBg.name = "Item Background";
        templateBg.transform.parent = itemTemplate.transform;

        AttachTransform(templateBg, 175, 20, 0.5f, 0.5f, 0, 0);
        toggle.targetGraphic = templateBg.AddComponent<Image>();

        ///////

        GameObject templateCheck = new GameObject();
        templateCheck.name = "Item Checkmark";
        templateCheck.transform.parent = itemTemplate.transform;

        AttachTransform(templateCheck, 15, 15, 0, 0.5f, 10, 0);
        var image3 = templateCheck.AddComponent<Image>();

        image3.sprite = Checkmark;
        image3.material = ArialFont.material;
        toggle.graphic = image3;

        ///////

        GameObject templateLabel = new GameObject();
        templateLabel.name = "Item Label";
        templateLabel.transform.parent = itemTemplate.transform;

        var textComponent2 = templateLabel.AddComponent<DDText>();
        dropdownComponent.itemText = textComponent2;

        textComponent2.font = font;
        textComponent2.alignment = TextAlignmentOptions.Left;
        textComponent2.fontSize = 16;

        StretchTransform(textComponent2.rectTransform);
    }

    private void StretchTransform(RectTransform rectTransform)
    {
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
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
