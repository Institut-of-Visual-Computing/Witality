using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingObjects : MonoBehaviour
{
    [HideInInspector]
    public int[] ids;
    public OpenCVTransRotMapper tracking_receiver;

    private void Start()
    {
        tracking_receiver.Init(transform);
    }
}
