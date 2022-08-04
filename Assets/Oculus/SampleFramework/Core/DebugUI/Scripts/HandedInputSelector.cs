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

    Transform t, index_tip, thumb_tip;
    void Update()
    {
        
        t = rightHand.PointerPose;
        t.position += m_CameraRig.position;
        t.RotateAround(m_CameraRig.position, Vector3.up, m_CameraRig.rotation.eulerAngles.y);

        if(!index_tip)
            index_tip = rightHand.transform.Find("Hand_IndexTip");
        if(!thumb_tip)
            thumb_tip = rightHand.transform.Find("Hand_ThumbTip");
        if (index_tip && thumb_tip)
        {
            float pinch = Mathf.Min(Vector3.Distance(index_tip.position, thumb_tip.position), 0.05f) / 0.05f;
            ray.startWidth = 0.01f + (1 - pinch) * 0.02f;
        }
        m_InputModule.rayTransform = t;


    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(index_tip.position, 0.1f);
        Gizmos.DrawSphere(thumb_tip.position, 0.1f);
    }
}
