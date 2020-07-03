using TMPro;
using UnityEngine;

class DDText : TextMeshProUGUI
{
    protected override void Start()
    {
        base.Start();
        fontSize = 16;
        rectTransform.offsetMin = new Vector2(20, 1);
        rectTransform.offsetMax = new Vector2(-10, -2);
    }
}