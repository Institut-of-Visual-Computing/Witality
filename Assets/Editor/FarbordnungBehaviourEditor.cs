using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
[CustomEditor(typeof(FarbordnungBehaviour))]

public class FarbordnungBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FarbordnungBehaviour _target = (FarbordnungBehaviour)target;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("White"))
        {
            _target.GetComponent<Rangordnung>().order_correct = _target.nr_weiß;
            _target.load_mats_farbordnung(0);
        }
        if (GUILayout.Button("Rosé"))
        {
            _target.GetComponent<Rangordnung>().order_correct = _target.nr_rose;
            _target.load_mats_farbordnung(1);
        }
        if (GUILayout.Button("Red"))
        {
            _target.GetComponent<Rangordnung>().order_correct = _target.nr_rot;
            _target.load_mats_farbordnung(2);
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Update Order Attacher"))
        {
            GameObject canvas = GameObject.Find("OrderAttacher/Canvas");
            canvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "hell";
            canvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "dunkel";
        }
    }
}
