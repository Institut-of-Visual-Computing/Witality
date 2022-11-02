using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleKeyInput : MonoBehaviour
{
    public KeyCode key;
    public bool activeAtStart = true; 
    public GameObject[] go;
    public Renderer[] rend;
    bool active;
    private void Start()
    {
        active = activeAtStart;
        set();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key)) 
        {
            active = !active;
            set();
        }
    }
    
    void set()
    {
        for (int i = 0; i < go.Length; i++)
        {
            go[i].SetActive(active);
        }
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].enabled = active;
        }
    }
}
