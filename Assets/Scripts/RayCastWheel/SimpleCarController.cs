using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class SimpleCarController : MonoBehaviour
{
    [Header("Car Specs")]
    public float WheelBase;
    public float TurnRadius;
    public float RearTrack;
    public float maxPower;

    [Header("Wheels")]
    public WheelColliderCust[] wheels;

    private InputManager im;

    private float ackermannAngleLeft;
    private float ackermannAngleRight;

    public Transform CM;
    // Start is called before the first frame update
    void Start()
    {
        im = GetComponent<InputManager>();
        GetComponent<Rigidbody>().centerOfMass = CM.localPosition;
    }

    private void Update()
    {
        if(im.steer > 0f) { // is turning right
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * im.steer;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * im.steer;
        }
        else if(im.steer < 0f){ // is turning left
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * im.steer;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * im.steer;
        } else { // is not turning
            ackermannAngleLeft = 0f;
            ackermannAngleRight = 0f;
        }

        foreach(WheelColliderCust w in wheels)
        {
            if (w.FrontLeft)
                w.SteerAngle = ackermannAngleLeft;
            if (w.FrontRight)
                w.SteerAngle = ackermannAngleRight;
            if (w.isDriveWheel)
                w.MotorTorque = maxPower * im.throttle;
            w.BrakeTorque = 500 * im.brakes;
        }
    }
}
