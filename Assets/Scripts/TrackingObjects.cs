using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingObjects : MonoBehaviour
{
    public int[] ids;

    private void Start()
    {
        GameObject.Find("TrackerReceiver").GetComponent<OpenCVTransRotMapper>().Init(transform);
    }
}
