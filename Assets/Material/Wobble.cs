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
    float rippleFallOff = 1;
    float rippleBuildUp = 4;
    public float rippleThreshold = 0.0001f;
    public float updateRate = 0.1f;
    float updateTimer = 0;
    float lastUpdateTime = 0;
    public float rippleLimit = 0.05f;
    public float rippleSpeed = 100f;
    float seedAdd;
    float rippleStrength;
    public bool testLimit = false;
    [Header("Read Only")]
    [Range(0, 1)]
    public float t;
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        seed = Random.Range(1, 10);
        seedAdd = 0;
        rend.material.SetFloat("_Seed",seed);
        lastPos = transform.position;
        lastUpdateTime = Time.time;
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

        updateTimer += Time.deltaTime;
        if (updateTimer > updateRate)
        {
            float deltaTime = Time.time - lastUpdateTime;
            // velocity
            velocity = (lastPos - transform.position) * deltaTime;

            //Ripple
            t += (velocity.magnitude > rippleThreshold ? rippleBuildUp : -rippleFallOff) * deltaTime;
            t = Mathf.Clamp01(t);
            if (testLimit)
                t = 1;
            seedAdd = Mathf.Lerp(0, rippleLimit, t);
            rippleStrength = Mathf.Lerp(0, 1, t);
            rend.material.SetFloat("_RippleStrength", rippleStrength);


            angularVelocity = transform.rotation.eulerAngles - lastRot;


            // keep last position
            lastPos = transform.position;
            lastRot = transform.rotation.eulerAngles;
            lastUpdateTime = Time.time;
            
            updateTimer = 0;
        }

        seed += seedAdd * rippleSpeed * Time.deltaTime;
        rend.material.SetFloat("_Seed", seed);


        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.z + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.x + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        
    }



}