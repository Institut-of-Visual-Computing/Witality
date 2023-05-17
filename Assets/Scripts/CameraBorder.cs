using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBorder : MonoBehaviour
{
    public Transform object_parent;
    public float threshold = -0.1f;
    public float intensityFallOff = 0.1f;
    public Transform lB, rB;
    public OpenCVTransRotMapper tracker;
    Plane left, right;
    public float[] distances;
    Renderer rendL, rendR;
    public Material mat;
    private void Start()
    {
        left = new Plane(lB.position, lB.position + lB.forward, lB.position + lB.right);
        right = new Plane(rB.position, rB.position + rB.forward, rB.position + rB.right);
        rendL = lB.GetComponent<Renderer>();
        rendR = rB.GetComponent<Renderer>();
        rendL.material = mat;
        rendR.material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        if (object_parent == null)
        {
            if(tracker.objects_parent != null)
                object_parent = tracker.objects_parent;
            return;
        }
        distances = new float[object_parent.childCount];
        for (int i = 0; i < object_parent.childCount; i++)
        {
            Vector3 pos = object_parent.GetChild(i).position;
            float dL = left.GetDistanceToPoint(pos);
            float dR = right.GetDistanceToPoint(pos);
            Debug.DrawLine(pos, left.ClosestPointOnPlane(pos),dL > 0 ? Color.green : Color.red);
            Debug.DrawLine(pos, right.ClosestPointOnPlane(pos), dR > 0 ? Color.green : Color.red);
            distances[i] = Mathf.Min(dL, dR);    
        }

        float alpha = Mathf.Clamp01(Maths.FloatMap(Mathf.Min(distances),threshold - intensityFallOff, threshold ,1,0));
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;


    }
}
