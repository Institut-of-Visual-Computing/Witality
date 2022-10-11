using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using TMPro;

[RequireComponent(typeof(Rangordnung))]
public class TrackingordnungBehaviour : MonoBehaviour
{
    
    public Transform tracking_obj;
    Rangordnung rang;
    public string[] scala_text;
    void Start()
    {
        rang = GetComponent<Rangordnung>();
        rang.order_correct = MenuSceneLoader.codes;
        rang.order = new int[rang.order.Length];
    }

}
