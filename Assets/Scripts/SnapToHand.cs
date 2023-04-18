using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToHand : MonoBehaviour
{


    Transform hand;
    Transform origin_parent;

    // Start is called before the first frame update
    void Start()
    {
        origin_parent = transform.parent;    
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent != origin_parent && transform.parent != hand.transform)
        {
            transform.parent = origin_parent;
        }
    }

    public void Snap(Transform p)
    {
        hand = p;
        transform.parent = p;
    }
    public void Unsnap()
    {
        transform.parent = origin_parent;
    }
}
