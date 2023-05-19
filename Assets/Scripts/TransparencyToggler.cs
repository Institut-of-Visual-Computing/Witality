using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyToggler : MonoBehaviour
{

    public UnityEngine.UI.Image background;

    public float[] alphas;
    public KeyCode[] keys;



    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                Color c = background.color;
                c.a = alphas[i];
                background.color = c;
            }
        }    
    }
}
