using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Oculus.Interaction;
using System.Linq;

public class OrderAttacher : MonoBehaviour
{
    public static Rangordnung rang;
    public int chosenInt = -1;
    public float minApllyRadius = 0.01f;
    public Transform handL, handR, buttons;
    public bool showing;
    public TextMeshProUGUI text,scalaLow,scalaHigh;
    private void Start()
    {
        setScala((int)MenuSceneLoader.task, MenuSceneLoader.subtask);
    }
    public void setScala(int task, int subtask)
    {
        switch (task)
        {
            case 1:
                scalaLow.text = "Neutral";
                scalaHigh.text = System.Enum.GetName(typeof(TaskChanger.Task_variante_G), subtask);
                break;
            case 2:
                scalaLow.text = "Neutral";
                scalaHigh.text = System.Enum.GetName(typeof(TaskChanger.Task_variante_A), subtask);
                break;
            default:
                scalaLow.text = "hell";
                scalaHigh.text = "dunkel";
                break;
        }
    }
    public void activated()
    {
        if (showing)
        {
            deactivate();
            return;
        }
        showing = true;
        handL = Maths.GetIndex(handL);
        handR = Maths.GetIndex(handR);
        for (int i = 0; i < buttons.childCount; i++)
        {
            buttons.GetChild(i).gameObject.SetActive(i < rang.obj_parent.childCount);
        }
    }
    
    public void setChosenInt(int i)
    {
        chosenInt = i;
        text.text = "Wähle das " + (i+1) + ". Element!";
    }
    private void Update()
    {
        if(chosenInt != -1)
        {
            
            int index = 0;
            float distance = Mathf.Min(
                Vector3.Distance(rang.obj_parent.GetChild(index).position,handL.position), 
                Vector3.Distance(rang.obj_parent.GetChild(index).position, handR.position));

            for (int i = 1; i < rang.obj_parent.childCount; i++)
            {
                float d = Mathf.Min(
                Vector3.Distance(rang.obj_parent.GetChild(i).position, handL.position),
                Vector3.Distance(rang.obj_parent.GetChild(i).position, handR.position));

                if (d < distance)
                {
                    distance = d;
                    index = i;
                }
            }

            if(distance < minApllyRadius)
            {
                rang.set_order(index, chosenInt);
                chosenInt = -1;
                text.text = "";
            }
        }
        for (int i = 0; i < rang.order.Length; i++)
        {
            setDone(rang.order[i] != 0, i);
        }
    }
    public void setDone(bool done, int i)
    {
        buttons.GetChild(i).Find("Visuals/ButtonVisual/ButtonPanel").GetComponent<RoundedBoxProperties>().setColor(done);
    }
    void deactivate()
    {
        chosenInt = -1;
        gameObject.SetActive(false);
        showing = false;
        text.text = "";
    }
    
}
#if (UNITY_EDITOR)
[CustomEditor(typeof(OrderAttacher))]
public class OrderAttacherEditor : Editor
{
    int t, st;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        OrderAttacher _target = ((OrderAttacher)target);

        EditorGUILayout.LabelField(System.Enum.GetName(typeof(TaskChanger.Task), t));
        t = EditorGUILayout.IntSlider(t, 0, 4);
        if (t == 1)
            st = EditorGUILayout.IntSlider(st, 0, 4);
        if (t == 2)
            st = EditorGUILayout.IntSlider(st, 0, 11);
        if (GUILayout.Button("Set"))
        {
            _target.setScala(t, st);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("All Done"))
        {
            for (int i = 0; i < _target.buttons.childCount; i++)
            {
                _target.setDone(true, i);
            }
        }
        if (GUILayout.Button("All Undone"))
        {
            for (int i = 0; i < _target.buttons.childCount; i++)
            {
                _target.setDone(false, i);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
