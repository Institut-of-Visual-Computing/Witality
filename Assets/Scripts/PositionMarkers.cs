using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TaskChanger.Task;
public class PositionMarkers : MonoBehaviour
{
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        Set(Task2GlasCount());
    }

    int Task2GlasCount()
    {
        switch (MenuSceneLoader.task)
        {
            case Aroma_Rangordnung:
            case Aroma_Erkennung:
                return 6;
            case Farbe_Rangordnung:
                return 0;
            case Geschmack_Rangordnung:
                return 7;
            case CATA:
                if (MenuSceneLoader.subtask == 0)
                    return 4;
                else
                    return 3;
            default:
                return 0;
        }
    }
    void Set(int count) 
    {
        text.text = "";
        for (int i = 0; i < count; i++)
        {
            text.text += "O";
        }
    }
}
