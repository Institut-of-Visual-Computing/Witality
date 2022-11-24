using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Oculus.Interaction;

public class Toggle_Gameobject : MonoBehaviour
{
    public Transform button;
    public bool ray_active { get; private set;}
    
    public void toggle(bool a)
    {
        gameObject.SetActive(a);
        ray_active = a;
        if(button != null)
            button.Find("Visuals/ButtonVisual/ButtonPanel").GetComponent<RoundedBoxProperties>().setColor(a);
    }
    public void toggle()
    {
        toggle(!gameObject.activeSelf);
    }
    
}


[CustomEditor(typeof(Toggle_Gameobject))]
public class Toggle_GameobjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Toggle_Gameobject _target = (Toggle_Gameobject)target;


        EditorGUILayout.LabelField("Ray is currently " + (_target.gameObject.activeSelf ? "" : "not ") + "active");
        if (GUILayout.Button("Toggle"))
        {
            

            _target.toggle();
        }
    }
}
