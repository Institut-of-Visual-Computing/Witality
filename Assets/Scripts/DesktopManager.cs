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
    FlaschenBehaviour[] bottles;

    string defaultText;

    private void Start()
    {
        defaultText = text.text;
        bottles = new FlaschenBehaviour[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            bottles[i] = objects[i].GetComponentInChildren<FlaschenBehaviour>();
        }
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
            if (Input.GetKeyDown(keyEvents[i]))
            {
                events[i].Invoke();
            }
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
            
            if (bottles[i] != null)
            {
                if(!set)
                    bottles[i].EndAnimation();
                bottles[i].SetPouring(set);
            }
        }
    }

    public void ResetRig()
    {
        OVRRig.position -= Maths.VectorNewY(OVRRig.position,0);
    }
}
