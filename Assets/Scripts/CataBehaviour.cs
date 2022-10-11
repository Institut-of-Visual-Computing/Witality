using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CataBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        load_mats_farbordnung();   
    }


    public void load_mats_farbordnung(int color = -1)
    {
        if (color == -1)
            color = TaskChanger.instance.subtask;
        string c = color == 0 ? "Weiﬂ" : "Rot";
        for (int x = 0; x < (color == 0 ? 4 : 3); x++)
        {
            transform.GetChild(x).Find("rotation/wine_glass_fill").GetComponent<Renderer>().material = Resources.Load( x == 0 ? "Cata Mats/Wasser" : "Cata Mats/" + c + "/" + x, typeof(Material)) as Material;
        }
        transform.GetChild(3).gameObject.SetActive(color == 0);
    }
}

[CustomEditor(typeof(CataBehaviour))]
public class CataBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CataBehaviour behaviour = (CataBehaviour)target;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("White"))
        {
            behaviour.load_mats_farbordnung(0);
        }
        if (GUILayout.Button("Red"))
        {
            behaviour.load_mats_farbordnung(1);
        }
        EditorGUILayout.EndHorizontal();
    }
}
