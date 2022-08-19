using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input.Filter;


[RequireComponent(typeof(HandFilter))]
public class HandFilterActivator : MonoBehaviour
{
    public HandFilter filter;

    // Update is called once per frame
    void Update()
    {
        if (!filter.enabled)
        {
            filter.enabled = true;
            Destroy(this);
        }
    }
}
