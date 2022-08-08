/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class HandedInputSelector : MonoBehaviour
{
    public Transform m_CameraRig;
    public OVRInputModule m_InputModule;
    public OVRHand rightHand;
    public LineRenderer ray;
    public float finger_min_distance = 0.1f;
    [Header("Min and Max")]
    public Vector2 ray_width = new Vector2(0, 0.02f);

    Transform t, index_tip, thumb_tip;
    void Update()
    {
        
        
        if (!index_tip)
            index_tip = getLastChild(getChildWithXInName(rightHand.transform.Find("Bones/Hand_WristRoot"), "Thumb"));

        if(!thumb_tip)
            thumb_tip = getLastChild(getChildWithXInName(rightHand.transform.Find("Bones/Hand_WristRoot"), "Index"));

        if (index_tip && thumb_tip)
        {
            float pinch = Mathf.Min(Vector3.Distance(index_tip.position, thumb_tip.position), finger_min_distance) / finger_min_distance;
            ray.startWidth = ray_width.x + (1 - pinch) * ray_width.y;

            
        }

        t = rightHand.PointerPose;
        t.position += m_CameraRig.position;
        t.RotateAround(m_CameraRig.position, Vector3.up, m_CameraRig.rotation.eulerAngles.y);
        
        m_InputModule.rayTransform = t;


    }

    private void OnDrawGizmos()
    {
        if (index_tip && thumb_tip)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(index_tip.position, 0.01f);
            Gizmos.DrawSphere(thumb_tip.position, 0.01f);

            Gizmos.DrawRay(index_tip.position, index_tip.right * 0.1f);
            Gizmos.DrawRay(thumb_tip.position, thumb_tip.right * 0.1f);
        }
    }

    Transform getLastChild(Transform t)
    {
        if (t == null)
            return null;
        Transform child = t;

        while(child.childCount > 0)
        {
            child = child.GetChild(0);
        }
        return child;
    }
    Transform getChildWithXInName(Transform t, string x)
    {
        if (t == null)
            return null;

        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).name.Contains(x))
            {
                return t.GetChild(i);
            }
        }
        return null;
    }
}
