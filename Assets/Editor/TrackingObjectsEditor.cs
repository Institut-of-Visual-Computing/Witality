using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackingObjects))]

public class TrackingObjectsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        GUILayout.Label("");
        TrackingObjects _target = (TrackingObjects)target;

        int objectCount = _target.transform.childCount;

        if (_target.ids == null)
            _target.ids = new int[objectCount];


        for (int i = 0; i < objectCount; i++)
        {
            _target.ids[i] = EditorGUILayout.IntSlider(_target.transform.GetChild(i).name + "'s \t ArUco ID:", _target.ids[i], 0, 9);
        }
        if (GUILayout.Button("Update"))
        {
            objectCount = _target.transform.childCount;

            for (int i = 0; i < objectCount; i++)
            {
                _target.transform.GetChild(i).Find("id").GetComponent<Renderer>().material = Resources.Load("ID Marker/Materials/" + _target.ids[i], typeof(Material)) as Material;
            }
        }
        GUILayout.Label("");

        for (int i = 0; i < objectCount; i++)
        {
            
            Transform obj = _target.transform.GetChild(i);
            if (GUILayout.Button("Rotate Object " + i + ": " + _target.transform.GetChild(i).name + "\t" +Mathf.RoundToInt((obj.rotation.eulerAngles.y)%360) + "°"))
            {
                
                for (int c = 0; c < obj.childCount; c++)
                {
                    Transform child = obj.GetChild(c);
                    if (child.name != "id")
                    {
                        child.RotateAround(obj.position, obj.up, -90);
                    }
                }
                obj.RotateAround(obj.position, obj.up, 90);
            }
        }
    }
    
}
