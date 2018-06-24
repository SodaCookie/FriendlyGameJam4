using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
	GameObject WorldReference;
	public float RotationSpeed = 0f;
	private float Angle = 0f;

	private void Start()
	{
		WorldReference = World.Instance.gameObject;
	}

	// Update is called once per frame
	void Update () {
		Angle += RotationSpeed * Time.deltaTime;
		Angle = Angle % 360;
		Vector3 upVector = transform.position - WorldReference.transform.position;
		Vector3 leftVector = Vector3.Cross(upVector, Camera.main.transform.position - transform.position);
		Vector3 forwardVector = Vector3.Cross(leftVector, upVector);
		transform.rotation = Quaternion.LookRotation(forwardVector, upVector) * Quaternion.AngleAxis(Angle, upVector);
	}
}
