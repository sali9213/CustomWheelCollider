using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{
    public float WheelBase;
    public float TurnRadius;
    public float RearTrack;
    private Wheels wheels;
    public float maxTurn = 20f;

    // Start is called before the first frame update
    void Start()
    {
        wheels = gameObject.GetComponent<Wheels>();
            
    }

    public float[] CalculateSteeringAngle(float input)
    {
      
        float ackermannAngleLeft = 0f;
        float ackermannAngleRight = 0f;

        if (input > 0f)
        { // is turning right
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * input;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * input;
        }
        else if (input < 0f)
        { // is turning left
            ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius - (RearTrack / 2))) * input;
            ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(WheelBase / (TurnRadius + (RearTrack / 2))) * input;
        }
        else
        { // is not turning
            ackermannAngleLeft = 0f;
            ackermannAngleRight = 0f;
        }

        return new float[] { ackermannAngleLeft, ackermannAngleRight };

    }
}
