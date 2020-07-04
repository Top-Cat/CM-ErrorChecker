using TMPro;
using UnityEngine;

class IText : TextMeshProUGUI
{
    // Don't ask, I wish this didn't exist
    protected override void Start()
    {
        base.Start();
        var sel = transform.parent.GetComponentInChildren<TMP_SelectionCaret>();
        var transform2 = sel.gameObject.GetComponent<RectTransform>();

        fontSize = 12;
        transform2.offsetMin = rectTransform.offsetMin = transform2.offsetMax = rectTransform.offsetMax = new Vector2(0, 0);
    }
}
