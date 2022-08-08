using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TaskChanger))]
public class TaskChangerEditor : Editor
{


    public override void OnInspectorGUI()
    {


        TaskChanger task = (TaskChanger)target;
        task.current_task = (TaskChanger.Task)EditorGUILayout.EnumPopup("Task:", task.current_task);

        switch (task.current_task)
        {
            case TaskChanger.Task.Farbe_Rangordnung:
                task.task_f = (TaskChanger.Task_variante_F)EditorGUILayout.EnumPopup("Variant:", task.task_f);
                break;
            case TaskChanger.Task.Geschmack_Rangordnung:
                task.task_g = (TaskChanger.Task_variante_G)EditorGUILayout.EnumPopup("Variant:", task.task_g);
                break;
            case TaskChanger.Task.Aroma_Erkennung:
            case TaskChanger.Task.Aroma_Rangordnung:
                task.task_a = (TaskChanger.Task_variante_A)EditorGUILayout.EnumPopup("Variant:", task.task_a);
                break;
            case TaskChanger.Task.CATA:
                task.task_c = (TaskChanger.Task_variante_C)EditorGUILayout.EnumPopup("Variant:", task.task_c);
                break;
        }

        base.OnInspectorGUI();
    }
}
