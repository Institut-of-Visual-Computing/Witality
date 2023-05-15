using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class WobbleAndRippleManager : MonoBehaviour
{

    public float maxWobble = 0.001f, wobbleSpeed = 1, recovery = 1, rippleLimit = 0.1f, rippleDensity = 15, rippleIntensity = 8, rippleSpeed = 100, rippleThreshold = 0.0001f;
    public bool testLimit,ripple;

    public Transform[] objects;


    // Start is called before the first frame update
    void Start()
    {
        execute();
    }


    public void execute()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            Wobble w = objects[i].GetComponent<Wobble>();
            w.MaxWobble = maxWobble;
            w.WobbleSpeed = wobbleSpeed;
            w.Recovery = recovery;
            w.rippleLimit = rippleLimit;
            w.testLimit = testLimit;
            w.rippleSpeed = rippleSpeed;
            w.rippleThreshold = rippleThreshold;

            Material m = objects[i].GetComponent<Renderer>().material;
            m.SetFloat("_RippleDensity", rippleDensity);
            m.SetFloat("_RippleIntensity", rippleIntensity);
            m.SetInt("_Ripple", ripple?1:0);
        }
    }
}

[CustomEditor(typeof(WobbleAndRippleManager))]
public class WobbleAndRippleManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Write"))
        {
            if (Application.isPlaying)
                ((WobbleAndRippleManager)target).execute();
            else
                Debug.Log("Writing only in Playmode.");
        }
    }
}
