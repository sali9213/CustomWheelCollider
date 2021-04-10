using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Tyres))]
[RequireComponent(typeof(Engine))]
[RequireComponent(typeof(Transmission))]
[RequireComponent(typeof(Aerodynamics))]
[RequireComponent(typeof(Brakes))]
[RequireComponent(typeof(Differential))]
[RequireComponent(typeof(Steering))]
[RequireComponent(typeof(Wheels))]
[RequireComponent(typeof(Guage))]
public class CarController : MonoBehaviour
{
    public InputManager im;
    //public Tyres ty;
    public Engine engine;
    public Transmission trans;
    public Aerodynamics aero;
    public Brakes brakes;
    public Differential diff;
    public Steering steer;
    public Wheels wheels;
    public Guage gauge;

    public Transform CM;
    public Rigidbody rb;

    // Initialisation
    private void Start()
    {
        im = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        //ty = GetComponent<Tyres>();
        engine = GetComponent<Engine>();
        trans = GetComponent<Transmission>();
        aero = GetComponent<Aerodynamics>();
        brakes = GetComponent<Brakes>();
        diff = GetComponent<Differential>();
        steer = GetComponent<Steering>();
        wheels = GetComponent<Wheels>();
        gauge = GetComponent<Guage>();

        if (CM)
        {
            rb.centerOfMass = CM.localPosition;
        }
    }

    private void Update()
    {
        // Set engine rpm according to wheel rpm, gear ratio and final drive ratio.
        engine.SetEngineRPM(trans.GetEngineRPM());
        float engineTorque = engine.GetTorque(im.throttle);
        float transTorque = trans.GetTorque(engineTorque);
        float[] wheelTorques = diff.DiffOutput(transTorque);
        float totalEngineBrake;
        float[] engineBrake = { 0f, 0f, 0f, 0f };

        if(rb.velocity.magnitude > 10f)
        {
            totalEngineBrake = engine.GetEngineBrakeTorque();
            engineBrake = wheels.GetEngineBraking(totalEngineBrake);
        }

        brakes.ApplyBrakes(im.brakes, engineBrake);
        wheels.ApplySteering(steer.CalculateSteeringAngle(im.steer));
        wheels.ApplyThrottleTorque(wheelTorques);
       
        // Apply aero drag
        aero.ApplyDrag();

        if (Input.GetButtonDown("ShiftUp")) trans.ShiftUp();
        if (Input.GetButtonDown("ShiftDown")) trans.ShiftDown();
    }
}
