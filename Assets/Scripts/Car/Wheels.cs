using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheels : MonoBehaviour
{
    //[FL, FR, BL, BR]
    //[SerializeField] private WheelCollider[] wc;
    [SerializeField] private WheelColliderCust[] wc;
    private List<int> drivenWheels = new List<int>();

    public enum dr
    {
        FWD,
        RWD,
        FOURWD
    }

    [SerializeField] private dr drivetrain = dr.RWD;
    [SerializeField] float wheelRadiusFrontInches = 12.9921f;
    [SerializeField] float wheelRadiusRearInches = 12.9921f;


    public List<int> DrivenWheels { get { return drivenWheels; } }
    public WheelColliderCust[] WC { get { return wc; } }
    public dr Drivetrain { get { return drivetrain; } }
    public float FrontRadius {  get { return wc[0].WheelRadius; } }
    public float RearRadius { get { return wc[2].WheelRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        wc = GetComponentsInChildren<WheelColliderCust>();

        if (drivetrain == dr.FWD || drivetrain == dr.FOURWD)
        {
            drivenWheels.Add(0);
            drivenWheels.Add(1);
        }
        if (drivetrain == dr.RWD || drivetrain == dr.FOURWD)
        {
            drivenWheels.Add(2);
            drivenWheels.Add(3);

        }

        for (int i = 0; i < wc.Length; i++)
        {
            if (i <= 1)
            {
                wc[i].WheelRadius = inchesToM(wheelRadiusFrontInches);
            } else
            {
                wc[i].WheelRadius = inchesToM(wheelRadiusRearInches);
            }
        }

    }

    // Update is called once per frame
    private float inchesToM(float inches)
    {
        return (inches * 2.54f) / 100;
    }

    public void ApplyThrottleTorque(float[] torques)
    {
        for(int i = 0; i < torques.Length; i++)
        {
            wc[i].MotorTorque = torques[i];
        }
    }

    public float[] GetEngineBraking(float totalEngineBrakeTorque)
    {
        float[] engineBrakeTorque = new float[2];

        if(drivetrain == dr.FWD)
        {
            engineBrakeTorque[0] = totalEngineBrakeTorque;

        } else if(drivetrain == dr.RWD)
        {
            engineBrakeTorque[1] = totalEngineBrakeTorque;

        } else if(drivetrain == dr.FOURWD)
        {
            engineBrakeTorque[0] = totalEngineBrakeTorque / 2;
            engineBrakeTorque[1] = totalEngineBrakeTorque / 2;
        }

        return engineBrakeTorque;
    }

    public void ApplySteering(float[] angles)
    {
        for (int i = 0; i < 2; i++)
        {
            wc[i].SteerAngle = angles[i];
            wc[i].gameObject.transform.localEulerAngles = new Vector3(0f, angles[i], 0f);
        }
    }
}
