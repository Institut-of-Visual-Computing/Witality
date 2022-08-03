using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBetweenGroupsWithCube : MonoBehaviour
{
    /*
     * Script needs a Object with a Child named "outer" with a box collider as trigger and a nested child called "inner"
     *
     * this
     * -outer   Trigger
     * --inner
     * 
     */

    public ToggleGameObject toggler;
    public float activationSpeed;
    Transform loadingCube;
    public bool inside;
    float scale = 0;
    bool changed = false;
    // Start is called before the first frame update
    void Start()
    {
        
        loadingCube = transform.Find("outer").Find("inner");
    }

    // Update is called once per frame
    void Update()
    {
        if(inside && scale <= 1)
        {
            scale += Time.deltaTime / activationSpeed;
            scale = Mathf.Min(scale, 1);
        }

        if(!inside && scale >= 0)
        {
            scale -= Time.deltaTime / activationSpeed;
            scale = Mathf.Max(scale, 0);
            
        }

        if(scale < 0.5f)
        {
            changed = false;
        }
        
        if (scale == 1 && !changed)
        {
            changed = true;
            if (toggler.active == 4)
            {
                toggler.activate(toggler.lastActiveRoom);
            }
            else
            {
                toggler.activate(4);
            }
        }

        loadingCube.localScale = Vector3.one * scale;

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if(other.tag=="Player")
            inside = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            inside = false;
    }

}
