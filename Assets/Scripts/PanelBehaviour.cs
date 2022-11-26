using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelBehaviour : MonoBehaviour
{

    public TextMeshProUGUI text, order;

    public Vector2 rect1, rect2;
    public RectTransform t;
    public int a;
    public float posY;
    // Update is called once per frame
    void Update()
    {
        a = 0;
        if (text.text != "")
            a++;
        if (order.text != "")
            a++;

        switch (a)
        {
            case 1:
                t.sizeDelta = rect1;
                t.localPosition = Vector3.zero;
                break;
            case 2:
                t.sizeDelta = rect2;
                t.localPosition = new Vector3(0,posY,0);
                break;
            default:
                t.sizeDelta = Vector2.zero;
                t.localPosition = Vector3.zero;
                break;
        }
        
    }
}
