using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopController : MonoBehaviour
{
    public Transform parentObject;
    public Transform hmd;
    public Transform optimalTransform;
    public float moveStepSize;
    public bool optimalPosVisible = true;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            optimalPosVisible = !optimalPosVisible;
            optimalTransform.GetChild(0).gameObject.SetActive(optimalPosVisible);

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            resetTarget();
            optimalPosVisible = false;
            optimalTransform.GetChild(0).gameObject.SetActive(optimalPosVisible);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
            move(-moveStepSize, Vector3.up);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            move(moveStepSize, Vector3.up);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            move(moveStepSize, Vector3.forward);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            move(-moveStepSize, Vector3.forward);
        if (Input.GetKeyDown(KeyCode.RightShift))
            move(-moveStepSize, Vector3.right);
        if (Input.GetKeyDown(KeyCode.RightControl))
            move(moveStepSize, Vector3.right);

    }
    public void resetTarget()
    {
        Vector3 neededTranslation = optimalTransform.position - hmd.position;

        parentObject.position += neededTranslation;

        Vector3 hmd_look = hmd.forward;
        hmd_look.y = 0;

        float angle = Vector3.Angle(hmd_look, optimalTransform.forward);
        parentObject.Rotate(Vector3.up, -angle);

        if (Vector3.Distance(optimalTransform.position, hmd.position) > 0.001f)
            resetTarget();
    }

    void move(float amount, Vector3 axis)
    {
        parentObject.position += axis * amount;
        optimalTransform.position += axis * amount;
    }

}
