using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
	//public float X = 0f;
	//public float Y = 10f;
	//public float Z = 10f;
	//// Start is called before the first frame update
	//void Start()
	//{

	//}

	//// Update is called once per frame
	//void Update()
	//{
	//    Transform target = gameObject.transform.parent.transform;
	//    Debug.Log(target.position);
	//    Vector3 targetPosition = new Vector3(target.position.x, this.transform.position.y, target.position.z);
	//    gameObject.transform.localPosition = new Vector3(X, Y, Z);
	//    gameObject.transform.LookAt(targetPosition);
	//}

	public Transform target;
	public float distance = 20.0f;
	public float height = 5.0f;
	public float heightDamping = 2.0f;

	public float lookAtHeight = 0.0f;

	public Rigidbody parentRigidbody;

	public float rotationSnapTime = 0.3F;

	public float distanceSnapTime;
	public float distanceMultiplier;

	private Vector3 lookAtVector;

	private float usedDistance;

	float wantedRotationAngle;
	float wantedHeight;

	float currentRotationAngle;
	float currentHeight;

	Quaternion currentRotation;
	Vector3 wantedPosition;

	private float yVelocity = 0.0F;
	private float zVelocity = 0.0F;

	void Start()
	{

		lookAtVector = new Vector3(0, lookAtHeight, 0);

	}

	void LateUpdate()
	{

		wantedHeight = target.position.y + height;
		currentHeight = transform.position.y;

		wantedRotationAngle = target.eulerAngles.y;
		currentRotationAngle = transform.eulerAngles.y;

		currentRotationAngle = Mathf.SmoothDampAngle(currentRotationAngle, wantedRotationAngle, ref yVelocity, rotationSnapTime);

		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		wantedPosition = target.position;
		wantedPosition.y = currentHeight;

		usedDistance = Mathf.SmoothDampAngle(usedDistance, distance + (parentRigidbody.velocity.magnitude * distanceMultiplier), ref zVelocity, distanceSnapTime);

		wantedPosition += Quaternion.Euler(0, currentRotationAngle, 0) * new Vector3(0, 0, -usedDistance);

		transform.position = wantedPosition;

		transform.LookAt(target.position + lookAtVector);

	}
}
