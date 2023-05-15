using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    public Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;
    float seed;
    float rippleSpeed = 1;
    public float rippleLimit = 0.05f;
    float seedAdd;
    float currentVelocity;
    float rippleStrength;
    public bool testLimit = false;
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        seed = Random.Range(1, 10);
        seedAdd = 0;
        rend.material.SetFloat("_Seed",seed);
        lastPos = transform.position;
    }
    private void Update()
    {
        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        // send it to the shader
        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        // velocity
        velocity = (lastPos - transform.position) * Time.deltaTime;

        //Ripple
        seedAdd = Mathf.SmoothDamp(seedAdd, velocity.magnitude > 0 ? rippleLimit : 0, ref currentVelocity, rippleSpeed);
        rippleStrength = Mathf.SmoothDamp(rippleStrength, velocity.magnitude > 0 ? 1 : 0, ref currentVelocity, rippleSpeed);
        seed += testLimit ? rippleLimit : seedAdd;
        rend.material.SetFloat("_Seed", seed);
        rend.material.SetFloat("_RippleStrength", testLimit ? 1 : rippleStrength);
        angularVelocity = transform.rotation.eulerAngles - lastRot;


        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.z + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.x + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }



}