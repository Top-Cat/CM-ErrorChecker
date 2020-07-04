using TMPro;
using UnityEngine;

class IText : TextMeshProUGUI
{
    // Don't ask, I wish this didn't exist
    protected override void Start()
    {
        base.Start();
        var sel = transform.parent.GetComponentInChildren<TMP_SelectionCaret>();
        if (sel != null)
        {
            var transform2 = sel.gameObject.GetComponent<RectTransform>();
            transform2.offsetMin = transform2.offsetMax = new Vector2(0, 0);
        }

        fontSize = 12;
         rectTransform.offsetMin = rectTransform.offsetMax = new Vector2(0, 0);
    }
}
