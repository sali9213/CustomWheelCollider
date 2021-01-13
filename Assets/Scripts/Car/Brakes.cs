using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brakes : MonoBehaviour
{
    [SerializeField] private float brakeBias = 0.6f;
    // Deceleration in m/s
    [SerializeField] private float maxDecel = 1f;

    private Wheels wheels;

    // Start is called before the first frame update
    void Start()
    {
        wheels = gameObject.GetComponent<Wheels>();

    }

    public void ApplyBrakes(float input, float[] engineBrake)
    {
        float decel = maxDecel * input;
        float carMass = gameObject.GetComponent<Rigidbody>().mass;

        float totalBrakeForce = carMass * decel;
        float frontBrakeForce = totalBrakeForce * brakeBias;
        float rearBrakeForce = totalBrakeForce - frontBrakeForce;

        float frontBrakeTorque = ((frontBrakeForce / 2) * wheels.FrontRadius) + engineBrake[0] / 2;
        float rearBrakeTorque = ((rearBrakeForce / 2) * wheels.RearRadius) + engineBrake[1] / 2;

        for(int i = 0; i < wheels.WC.Length; i++)
        {
            if(wheels.WC[i].GetGroundHit(out WheelHit hit))
            {
                if (hit.forwardSlip > -wheels.WC[i].forwardFriction.extremumSlip)
                {
                    if (i < 2)
                    {
                        wheels.WC[i].brakeTorque = frontBrakeTorque;
                    }
                    else
                    {
                        wheels.WC[i].brakeTorque = rearBrakeTorque;
                    }
                } else
                {
                    wheels.WC[i].brakeTorque = 0f;
                }
            }

        }

    }
}
