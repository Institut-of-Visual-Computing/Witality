using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrackingObjects : MonoBehaviour
{
    [HideInInspector]
    public int[] ids;
    public OpenCVTransRotMapper tracking_receiver;

    private void Start()
    {
        tracking_receiver.Init(transform);
        for (int i = 0; i < MenuSceneLoader.codes.Length; i++)
        {
            if (transform.GetChild(i).name == "CalibrationCube")
                continue;
            setText(i, MenuSceneLoader.codes[i].ToString("000."));
        }
    }

    public void setText(int child, string value, string name = "text")
    {
        transform.GetChild(child).Find("Canvas/" + name).GetComponent<TextMeshProUGUI>().text = value;
    }
}
