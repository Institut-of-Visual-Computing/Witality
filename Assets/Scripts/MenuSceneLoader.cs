using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuSceneLoader : MonoBehaviour
{
    public static TaskChanger.Task task = TaskChanger.Task.Farbe_Rangordnung;
    public static int subtask = 0;
    public static int environment = 1;
    public static int probandID = 0;
    public static Vector3 calibPosition_Hmd, calibPosition_Camera;
    public static Quaternion calibRotation_Hmd, calibRotation_Camera;
    public static int[] codes;
    public static bool demographic, ipq, english = false;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);   
    }

    
}

[CustomEditor(typeof(MenuSceneLoader))]
public class MenuSceneLoaderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Task:\t\t" + MenuSceneLoader.task);
        EditorGUILayout.LabelField("Subtask:\t" + MenuSceneLoader.subtask);
        EditorGUILayout.LabelField("Environment:\t" + MenuSceneLoader.environment);
        EditorGUILayout.LabelField("Proband ID:\t" + MenuSceneLoader.probandID);
        EditorGUILayout.LabelField("Codes:");
        if(MenuSceneLoader.codes != null)
        for (int i = 0; i < MenuSceneLoader.codes.Length; i++)
        {
            EditorGUILayout.LabelField("\t"+i+":\t" + MenuSceneLoader.codes[i]);
        }
        else
            EditorGUILayout.LabelField("\t\tNot defined");
        EditorGUILayout.LabelField("Demographic:\t" + MenuSceneLoader.demographic);
        EditorGUILayout.LabelField("IPQ:\t\t" + MenuSceneLoader.ipq);
    }
}
