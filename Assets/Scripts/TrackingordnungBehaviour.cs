using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using TMPro;

[RequireComponent(typeof(Rangordnung))]
public class TrackingordnungBehaviour : MonoBehaviour
{
    
    public Transform tracking_obj;
    Rangordnung rang;
    public string[] scala_text;
    public Transform orderAttacherCanvas;
    void Start()
    {
        rang = GetComponent<Rangordnung>();
        rang.order_correct = MenuSceneLoader.codes;
        load(MenuSceneLoader.subtask);
    }
    public void load(int i)
    {
        orderAttacherCanvas.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Neutral";
        orderAttacherCanvas.GetChild(2).GetComponent<TextMeshProUGUI>().text = scala_text[i];
    }

}


[CustomEditor(typeof(TrackingordnungBehaviour))]
public class TrackingordnungBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TrackingordnungBehaviour _target = (TrackingordnungBehaviour)target;
        
        GUILayout.BeginHorizontal();
        for (int i = 0; i < _target.scala_text.Length; i++)
        {
            if (GUILayout.Button(_target.scala_text[i]))
            {
                _target.load(i);
            }
            if (i % 4 == 3)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load Aroma"))
        {
            _target.scala_text = System.Enum.GetNames(typeof(TaskChanger.Task_variante_A));
        }
        if (GUILayout.Button("Load Geschmack"))
        {
            _target.scala_text = System.Enum.GetNames(typeof(TaskChanger.Task_variante_G));
        }
        GUILayout.EndHorizontal();
    }
}
