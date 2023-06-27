using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class DesktopManager : MonoBehaviour
{
    public GameObject[] objects;
    public KeyCode[] keys;
    public UnityEvent[] events;
    public KeyCode[] keyEvents;
    public TextMeshProUGUI text;
    public Transform OVRRig;


    string defaultText;

    private void Start()
    {
        defaultText = text.text;
    }
    void Update()
    {

        text.text = defaultText+"\n";
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                objects[i].SetActive(!objects[i].activeSelf);
                SetEinschenken(true);
            }
            text.text += ApplyFormat(keys[i].ToString()) + ":\t" + objects[i].name + " ist " + (objects[i].activeSelf ? "aktiv." : "ausgeblendet.") + "\n";
        }
        for (int i = 0; i < keyEvents.Length; i++)
        {
            events[i].Invoke();
        }

        string ApplyFormat(string input)
        {
            string output = input;
            if (input.Length > 5 && input.Substring(0, 5) == "Alpha")
                output = input.Substring(5);
            return output;
        }
    }

    public void SetEinschenken(bool set)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            Animator a = objects[i].GetComponentInChildren<Animator>();
            if (a != null)
            {
                a.SetBool("loop", set);
            }
        }
    }

    public void ResetRig()
    {
        OVRRig.position -= Maths.VectorNewY(OVRRig.position,0);
    }
}
