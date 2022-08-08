using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EventCube : MonoBehaviour
{
    FarbordnungBehaviour parent;

    public float activationSpeed = 2;
    public UnityEvent ActivationEvent;

    Transform loadingCube;
    public bool inside;
    List<Collider> inside_cld;
    float scale = 0;
    bool changed = false;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<FarbordnungBehaviour>();
        loadingCube = transform.Find("inner");
        inside_cld = new List<Collider>();
    }
    private void Update()
    {
        inside = inside_cld.Count > 0;

        if (inside && scale <= 1)
        {
            scale += Time.deltaTime / activationSpeed;
            scale = Mathf.Min(scale, 1);
        }

        if (!inside && scale >= 0)
        {
            scale -= Time.deltaTime / activationSpeed;
            scale = Mathf.Max(scale, 0);

        }

        if (changed && !inside)
        {
            scale = 0;
            changed = false;
        }

        if (scale == 1 && !changed)
        {
            changed = true;
            ActivationEvent.Invoke();

        }

        loadingCube.localScale = Vector3.one * scale;
    }

    

    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponentInParent<OVRHand>())
            inside_cld.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<OVRHand>())
            inside_cld.Remove(other);
    }
}
