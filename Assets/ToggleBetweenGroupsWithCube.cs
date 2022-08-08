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
    List<Collider> inside_cld;
    // Start is called before the first frame update
    void Start()
    {
        
        loadingCube = transform.Find("inner");
        inside_cld = new List<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        inside = inside_cld.Count > 0;

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

        if(changed && !inside)
        {
            scale = 0;
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

        if (other.GetComponentInParent<OVRHand>())
            inside_cld.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<OVRHand>())
            inside_cld.Remove(other);
    }
   

}
