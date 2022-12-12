using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.Linq;

public class Rangordnung : MonoBehaviour
{
    public Transform obj_parent;
    public TextMeshProUGUI order_text;
    public GameObject submit_Button;
    [Header("Read Only")]
    public int[] order;
    public int[] order_correct;
    public static bool isValid = false;
    public static Rangordnung instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }
    public string order_string(int[] array)
    {
        bool valid = true;
        string o = "";
        for (int i = 0; i < array.Length; i++)
        {
            if(array[i] == 0)
            {
                valid = false;
            }
            else
            {
                o += array[i].ToString("000.") + "-";
            }
            
        }
        o = o.Substring(0, Mathf.Max(0, o.Length - 1));
        isValid = valid;
        updateValidButton();
        return o;
    }
    public void apply_order_text()
    {
        for (int i = 0; i < order_correct.Length; i++)
        {
            string value = (System.Array.IndexOf(order, order_correct[i]) + 1) + ".";
            if (value == "0.")
                value = "";
            setText(i, value, "order");
        }
        order_text.text = order_string(order);
    }
    public void apply_text()
    {
        if(MenuSceneLoader.codes != null)
            order_correct = MenuSceneLoader.codes;

        for (int i = 0; i < order_correct.Length; i++)
        {
            setText(i, order_correct[i].ToString("000."));
        }
    }
    public void setText(int child, string value ,string name = "text")
    {
        obj_parent.GetChild(child).Find("Canvas/" + name).GetComponent<TextMeshProUGUI>().text = value == "000" ? "Wasser" : value;
    }
    public string getText(int child, string name = "text")
    {
        string text = obj_parent.GetChild(child).Find("Canvas/" + name).GetComponent<TextMeshProUGUI>().text;
        return text == "Wasser" ? "000" : text;
    }
    public void activateAttacher()
    {
        OrderAttacher.rang = this;
    }
    public void set_order(int child, int pos)
    {
        if (order.Length != order_correct.Length)
            order = new int[order_correct.Length];
        int id = int.Parse(getText(child));
        int prevIndex = System.Array.IndexOf(order, id);
        if (prevIndex >= 0)
        {
            Debug.Log(prevIndex + " " + pos + " " + child);
            int otherId = order[pos];
            order[prevIndex] = otherId;
        }
        order[pos] = id;
        apply_order_text();
    }
    public void updateValidButton()
    {
        submit_Button.SetActive(isValid);
    }

    public void readFromPosition()
    {
        //created sorted list
        Transform[] sorted = new Transform[order.Length];
        for (int i = 0; i < sorted.Length; i++)
        {
            Transform mostLeft = null;

            for (int j = 0; j < order.Length; j++)
            {
                Transform c = obj_parent.GetChild(j);
                if (mostLeft == null)
                    if (!sorted.Contains(c))
                        mostLeft = c;
                    else
                        continue;


                if (!sorted.Contains(c) && c.position.x < mostLeft.position.x)
                    mostLeft = c;
            }
            sorted[i] = mostLeft;
        }
        //Debug
        for (int i = 0; i < sorted.Length; i++)
        {
            Debug.Log(i + ": " + sorted[i].position.x);
        }
        //apply list onto order
        for (int i = 0; i < sorted.Length; i++)
        {
            for (int j = 0; j < obj_parent.childCount; j++)
            {
                if (obj_parent.GetChild(j) == sorted[i])
                {
                    set_order(j, i);
                    break;
                }
            }
        }

    }

}



[CustomEditor(typeof(Rangordnung))]
public class RangordnungEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Rangordnung _target = (Rangordnung)target;

        if (GUILayout.Button("Update Order"))
        {
            _target.apply_text();
            _target.apply_order_text();
        }
    }
}
