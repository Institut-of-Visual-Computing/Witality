using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlaschenBehaviour : MonoBehaviour
{
    public Animator anim;
    public ParticleSystem particle;

    public void SetPouring(bool active)
    {
        if (active)
            particle.Play();
        else
            particle.Stop();
    }

    public void EndAnimation()
    {
        anim.Play("Einschenken_e");
    }

}
