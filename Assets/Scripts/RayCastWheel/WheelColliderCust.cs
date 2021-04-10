using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WheelColliderCust : MonoBehaviour
{
    private Rigidbody rb;
    private Pacejka pk;
    private Suspension sp;

    public float SuspensionDistance { get { return sp.springLength; } }
    public Vector3 centre { get { return transform.position - (transform.up * sp.springLength); } }

    // These need to be deleted
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
    [SerializeField]
    private float SpringForce;
    private Vector3 suspensionForce;
    private Vector3 wheelVelocityLS;


    [Header("Wheel")]
    public float WheelRadius;
    public float MotorTorque;
    public float BrakeTorque;
    public float SteerAngle;
    public float WheelMass;

    private float slipRatio;
    public float slipAngle { get; private set; }
    private float tractionForce;
    [SerializeField]
    private float tractionTorque;
    private float totalTorque;
    private float wheelInertia;
    private float angularAcceleration;
    private float angularVelocity;
    private Vector2 wheelVel;
    private Vector2 forward = new Vector2(0f, 1f);
    [SerializeField]
    private float Fx;
    [SerializeField]
    private float Fy;
    public float RPM { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        pk = new Pacejka();
        sp = new Suspension(RestLength, SpringTravel, SpringStiffness, DamperStiffness);

        rb = transform.root.GetComponent<Rigidbody>();

        wheelInertia = WheelMass * (WheelRadius * WheelRadius) / 2;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y + SteerAngle, transform.localRotation.z);
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, sp.maxLength + WheelRadius)) {
            float suspensionForceMag = sp.CalculateSuspensionForce(hit, WheelRadius);
            suspensionForce = suspensionForceMag * hit.normal;

            wheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
            wheelVel = new Vector2(wheelVelocityLS.x, wheelVelocityLS.z);

            tractionTorque = tractionForce * WheelRadius;
            totalTorque = MotorTorque - tractionTorque;

            if (wheelVelocityLS.magnitude <5f)
            {
                Fy = wheelVelocityLS.x * sp.springForce;
                SpringForce = sp.springForce;
                //Fx = totalTorque * WheelRadius * sp.springForce/1000f;
            } else
            {
                slipAngle = Vector2.Angle(forward, wheelVel) * Mathf.Sign(wheelVel.x);
                Fy = pk.CalcLateralF(suspensionForceMag / 1000f, slipAngle, 0f);
            }

            slipRatio = (((angularVelocity * WheelRadius) - wheelVel.y) / Mathf.Abs(wheelVel.y)) * 100;

            if (float.IsNaN(slipRatio))
            {
                slipRatio = 0;
            }

            Fx = pk.CalcLongitudinalF(sp.springForce / 1000f, slipRatio);

            angularAcceleration = totalTorque / wheelInertia;
            angularVelocity += angularAcceleration * Time.deltaTime;

            if (BrakeTorque != 0f)
            {
                BrakeTorque = Mathf.Sign(RPM) * BrakeTorque;

                float angularAccelerationBraking = BrakeTorque / wheelInertia;
                float deltaVelocityBraking = angularAccelerationBraking * Time.deltaTime;

                if (Mathf.Abs(angularVelocity) >= Mathf.Abs(deltaVelocityBraking))
                {
                    angularVelocity -= deltaVelocityBraking;
                }
                else
                {
                    angularVelocity = 0f;
                }
            }

            RPM = angularVelocity * (30 / Mathf.PI);



            if (float.IsNaN(Fx))
            {
                Fx = 0; 
            }

            if (angularVelocity == 0f && BrakeTorque != 0f)
            {
                tractionForce = 0f;
            }
            else
            {
                tractionForce = Fx;
            }

            Debug.DrawLine(hit.point, hit.point + transform.TransformDirection(new Vector3(wheelVel.x, 0f, wheelVel.y)));

            float forceImpactNormal = Mathf.Cos(Vector3.Angle(hit.normal, transform.up) * Mathf.PI / 180f);
            forceImpactNormal = Mathf.Clamp(forceImpactNormal, 0f, 1f);

            suspensionForce = forceImpactNormal * suspensionForce;

            //Apply traction and suspension forces
            rb.AddForceAtPosition((Fx * transform.forward) + (Fy * -transform.right), hit.point);
            rb.AddForceAtPosition(suspensionForce, transform.position);

        } else
        {
            sp.ExtendMax();
        }
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Handles.color = Color.green;

            Vector3 wheelCentre = transform.position - transform.up * sp.springLength;
            Vector3 normal = transform.TransformDirection(new Vector3(1, 0, 0));
            Vector3 gizmoTarget = transform.position + (transform.rotation * new Vector3(0, -1, 0)) * (sp.springLength + WheelRadius);

            Gizmos.DrawLine(transform.position, gizmoTarget);
            Gizmos.DrawSphere(gizmoTarget, 0.05f);
            Handles.DrawWireDisc(wheelCentre, normal, WheelRadius);
        } else
        {
            Gizmos.color = Color.red;
            Handles.color = Color.green;

            Vector3 wheelCentre = transform.position - transform.up * RestLength;
            Vector3 normal = transform.TransformDirection(new Vector3(1, 0, 0));
            Vector3 gizmoTarget = transform.position + (transform.rotation * new Vector3(0, -1, 0)) * (RestLength + WheelRadius);

            Gizmos.DrawLine(transform.position, gizmoTarget);
            Gizmos.DrawSphere(gizmoTarget, 0.05f);
            Handles.DrawWireDisc(wheelCentre, normal, WheelRadius);
        }
        
    }
#endif

}
