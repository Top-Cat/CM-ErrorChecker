using TMPro;
using UnityEngine;

class DDText : TextMeshProUGUI
{
    // Don't ask, I wish this didn't exist
    protected override void Start()
    {
        base.Start();
        fontSize = 12;
        rectTransform.offsetMin = new Vector2(20, 1);
        rectTransform.offsetMax = new Vector2(-10, -2);
    }
}
