using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Clean the fuck up
public class CustomWheel : MonoBehaviour
{
    private Rigidbody rb;
    private Pacejka pk;

    public bool FrontLeft;
    public bool FrontRight;
    public bool RearLeft;
    public bool RearRight;

    public bool isDriveWheel = false;

    [Header("Suspension")]
    public float RestLength;
    public float SpringTravel;
    public float SpringStiffness;
    public float DamperStiffness;

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springForce;
    private float springVelocity;
    private float damperForce;
    private Vector3 suspensionForce;
    private Vector3 wheelVelocityLS;
    [SerializeField]
    private float Fx;
    [SerializeField]
    private float Fy;

    [Header("Wheel")]
    public float WheelRadius;
    public float MotorTorque;
    public float BrakeTorque;
    public float SteerAngle;
    public float WheelMass;

    [SerializeField]
    private float lowSpeedForce;
    [SerializeField]
    private float RPM;
    [SerializeField]
    private float slipRatio;
    private float slipAngle;
    [SerializeField]
    private float tractionForce;
    [SerializeField]
    private float tractionTorque;
    [SerializeField]
    private float totalTorque;

    private float WheelInertia;
    [SerializeField]
    private float AngularAcceleration;
    [SerializeField]
    private float Angularvelocity;
    private Vector2 wheelVel;
    Vector2 forward = new Vector2(0f, 1f);

    [Header("Lateral Friction")]
    public AnimationCurve frictionCurve;
    public float Bx;
    public float Cx;
    public float Dx;
    public float Ex;

    [Header("Longitudinal Friction")]
    public AnimationCurve longitudinalFrictionCurve;
    public float By;
    public float Cy;
    public float Dy;
    public float Ey;

    // Start is called before the first frame update
    void Start()
{
    pk = new Pacejka();
    rb = transform.root.GetComponent<Rigidbody>();

    WheelInertia = WheelMass * (WheelRadius * WheelRadius) / 2;

    minLength = RestLength - SpringTravel;
    maxLength = RestLength + SpringTravel;
}

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y + SteerAngle, transform.localRotation.z);
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + WheelRadius)) {
            lastLength = springLength;
            springLength = hit.distance - WheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (lastLength - springLength) / Time.deltaTime;
            springForce = SpringStiffness * (RestLength - springLength);
            damperForce = DamperStiffness * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;

            wheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
            wheelVel = new Vector2(wheelVelocityLS.x, wheelVelocityLS.z);

            if (transform.InverseTransformDirection(rb.GetPointVelocity(hit.point)).magnitude > 0)
            {
                slipAngle = Vector2.Angle(forward, wheelVel) * Mathf.Sign(wheelVel.x);
                //Fy = CalculateLateralFrictionForce();
                Fy = pk.CalcLateralF(springForce / 1000f, slipAngle, 0f);
            } else
            {
                Fy = wheelVelocityLS.x * springForce;
            }

            Debug.DrawLine(hit.point, hit.point + transform.TransformDirection(new Vector3(wheelVel.x, 0f, wheelVel.y)));

            tractionTorque = tractionForce * WheelRadius;
            totalTorque = MotorTorque - BrakeTorque - tractionTorque;
            AngularAcceleration = totalTorque / WheelInertia;
            Angularvelocity += AngularAcceleration * Time.deltaTime;
            RPM = Angularvelocity * (30 / Mathf.PI);

            if (Mathf.Abs(wheelVel.y) == 0)
            {
                slipRatio = 0;
            }
            else
            {
                slipRatio = (((Angularvelocity * WheelRadius) - wheelVel.y) / Mathf.Abs(wheelVel.y)) * 100;
            }


            //Fx = CalculateLongitudinalFrictionForce();
            Fx = pk.CalcLongitudinalF(springForce / 1000f, slipRatio);

            if (float.IsNaN(Fx))
            {
                tractionForce = 0f;
            }
            else
            {
                tractionForce = Fx;
            }

            // Apply traction and suspension forces
            rb.AddForceAtPosition((Fx * transform.forward) + (Fy * -transform.right), hit.point);
            rb.AddForceAtPosition(suspensionForce, transform.position);
            
        } else
        {
            springLength = maxLength;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 gizmoTarget = transform.position + (transform.rotation * new Vector3(0, -1, 0)) * (springLength + WheelRadius);
        Gizmos.DrawLine(transform.position, gizmoTarget);
        Gizmos.DrawSphere(gizmoTarget, 0.05f);
    }

    //private float CalculateLongitudinalFrictionForce()
    //{
    //    return springForce * Dy * Mathf.Sin(Cy * (Mathf.Atan((By * slipRatio) - Ey * (By * slipRatio - (Mathf.Atan(By * slipRatio))))));
    //}

    //private float CalculateLateralFrictionForce()
    //{
    //    return springForce * Dx * Mathf.Sin(Cx * (Mathf.Atan(Bx * slipAngle - Ex * (Bx * slipAngle - (Mathf.Atan(Bx * slipAngle))))));
    //}

    //private void CalculateLateralCurve()
    //{
    //    for(int i = -180; i <= 180; i++)
    //    {
    //        float Fx = 4900f * Dy * Mathf.Sin(Cy * (Mathf.Atan(By * i - Ey * (By * i - (Mathf.Atan(By * i))))));
    //        frictionCurve.AddKey(i, Fx);
    //    }
    //}

    //private void CalculateLongitudinalCurve()
    //{
    //    for(int i = 0; i <= 100; i++)
    //    {
    //        float F = 4900f * Dy * Mathf.Sin(Cy * (Mathf.Atan((By * i) - Ey * (By * i - (Mathf.Atan(By * i))))));
    //        longitudinalFrictionCurve.AddKey(i, F);

    //    }
    //}
    
}
