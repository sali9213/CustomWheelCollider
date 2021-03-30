using UnityEngine;
using System.Collections;

// This script ensures that the wheels follow the position of the wheel colliders making it look like the wheel has suspension

public class WheelSync : MonoBehaviour
{

    public WheelColliderCust wheelC;
    private Vector3 wheelCCenter;
    private RaycastHit hit;

    // This is to flip the direction of rotation of the wheels because of a problem with the model used during development.
    // This can be removed as long as the model has been created properly.
    [SerializeField] private bool flipRotation;

    private void Start()
    {
        wheelC = transform.parent.GetComponent<WheelColliderCust>();
    }

    void Update()
    {
        //wheelCCenter = wheelC.transform.TransformPoint(wheelC.centre);
        wheelCCenter = wheelC.centre;

        if (Physics.Raycast(wheelCCenter, -wheelC.transform.up, out hit, wheelC.SuspensionDistance + wheelC.WheelRadius))
        {
            transform.position = hit.point + (wheelC.transform.up * wheelC.WheelRadius);
        }
        else
        {
            transform.position = wheelCCenter - (wheelC.transform.up * wheelC.SuspensionDistance);
        }

        int rotationDirection = flipRotation ? -1 : 1;

        gameObject.transform.Rotate(0, wheelC.RPM / 60 * 360 * rotationDirection * Time.deltaTime, 0);

    }


}