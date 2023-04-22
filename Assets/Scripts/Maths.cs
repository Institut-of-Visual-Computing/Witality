using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maths
{
    public static Vector3 Avg_v(List<Vector3> v)
    {
        if (v.Count == 0)
            return Vector3.zero;
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < v.Count; i++)
        {
            sum += v[i];
        }
        return sum / v.Count;
    }
    public static Vector3 Avg_v(Vector3[] v)
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < v.Length; i++)
        {
            sum += v[i];
        }
        return sum / v.Length;
    }
    public static Vector3 Median_v(List<Vector3> v)
    {
        if (v.Count == 0)
            return Vector3.zero;
        Vector3 avg = Avg_v(v);
        float d = float.MaxValue;
        Vector3 min = Vector3.zero;
        for (int i = 0; i < v.Count; i++)
        {
            float dis = Vector3.Distance(v[i], avg);
            if (dis < d)
            {
                d = dis;
                min = v[i];
            }
        }
        return min;
    }
    public static Quaternion Avg_q(Quaternion[] q)
    {
        if (q == null || q.Length == 0)
            return new Quaternion();
        Quaternion avg = q[0];
        for (int i = 1; i < q.Length; i++)
        {
            avg = Quaternion.Lerp(q[i], avg, 1 / (i + 1));
        }
        return avg;
    }
    public static Quaternion Avg_q(List<Quaternion> q)
    {
        if (q == null || q.Count == 0)
            return new Quaternion();
        Quaternion avg = q[0];
        for (int i = 1; i < q.Count; i++)
        {
            avg = Quaternion.Lerp(q[i], avg, 1 / (i + 1));
        }
        return avg;
    }
    public static Quaternion Median_q(List<Quaternion> q)
    {
        if (q.Count == 0)
            return new Quaternion();
        Quaternion avg = Avg_q(q);
        float d = float.MaxValue;
        Quaternion min = new Quaternion();
        for (int i = 0; i < q.Count; i++)
        {
            float dis = Quaternion.Angle(q[i], avg);
            if (dis < d)
            {
                d = dis;
                min = q[i];
            }
        }
        return min;
    }
    public static Transform GetThumb(Transform hand)
    {
        Transform tip = hand.Find("Bones/Hand_WristRoot/Hand_Thumb0/Hand_Thumb1/Hand_Thumb2/Hand_Thumb3/Hand_ThumbTip");
        if (tip == null)
            return hand;
        while (tip.childCount > 0)
            tip = tip.GetChild(0);
        return tip;
    }
    public static Transform GetIndex(Transform hand)
    {
        Transform tip = hand.Find("Bones/Hand_WristRoot/Hand_Index1/Hand_Index2/Hand_Index3/Hand_IndexTip");
        if (tip == null)
            return hand;
        // hand > Bones > Hand_WristRoot > Hand_Index[1,2,3,Tip]
        while (tip.childCount > 0)
            tip = tip.GetChild(0);
        return tip;
    }
    public static Transform GetWrist(Transform hand)
    {
        return hand.Find("Bones/Hand_WristRoot");
    }
    public static Vector3 Avg_v(Vector3 a, Vector3 b)
    {
        return (a + b) / 2;
    }
    public static Vector3 ZeroVector(Vector3 v, bool x, bool y, bool z)
    {
        return new Vector3(x ? 0 : v.x, y ? 0 : v.y, z ? 0 : v.z);
    }
    public static Vector3 VectorNewY(Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }
}
