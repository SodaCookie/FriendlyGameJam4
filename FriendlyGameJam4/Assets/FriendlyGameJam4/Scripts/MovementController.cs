using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	public GameObject Visual;
	public GameObject ForwardReference;
	public bool isPoint = false;
	public Quaternion Rotation = Quaternion.identity;
	private float _height = 0f;
	public float Height {
		get {
			return _height;
        }
		set {
			_height = value;
			Visual.transform.localPosition = new Vector3(0, _height, 0);
		}
	}

	private void OnEnable()
	{
		World.Instance.Controllers.Add(this);
	}


	private void OnDisable()
	{
		World.Instance.Controllers.Remove(this);
	}

	// Update is called once per frame
	void Update () {
		// Perform Raycast for height
		UpdatePosition();
	}

	public void UpdatePosition() {
		// Perform Raycast for height
        Vector3 inward = Rotation * Vector3.down;
        Vector3 position = Rotation * Vector3.up * 40;
        RaycastHit hitInfo;

		if (Physics.Raycast(position, inward, out hitInfo, 40, LayerMask.GetMask("Planet")))
        {
            float height = (hitInfo.point - World.Instance.gameObject.transform.position).magnitude;
            transform.position = Rotation * Vector3.up * height;
            if (!isPoint)
            {
                Vector3 upAxis = transform.position - World.Instance.gameObject.transform.position;
                Vector3 leftAxis = Vector3.Cross(ForwardReference.transform.forward, upAxis);
                Vector3 forwardAxis = Vector3.Cross(upAxis, leftAxis);
				var oldAngle = transform.rotation;
				var newAngle = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forwardAxis, upAxis), 10f * Time.deltaTime);
				if (Quaternion.Angle(oldAngle, newAngle) > 0.1f) {
					transform.rotation = newAngle;
				}
			}
        }
	}

	public void Move(Vector2 displacement) {
		if (!isPoint) {
			Vector3 upAxis = transform.position - World.Instance.gameObject.transform.position;
            Vector3 leftAxis = Vector3.Cross(ForwardReference.transform.forward, upAxis);
            Vector3 forwardAxis = Vector3.Cross(upAxis, leftAxis);
            Rotation = Quaternion.AngleAxis(-displacement.x / World.Instance.WorldScale, leftAxis) * Rotation;
            Rotation = Quaternion.AngleAxis(displacement.y / World.Instance.WorldScale, forwardAxis) * Rotation;
		}
		else {
            Rotation = Quaternion.AngleAxis(-displacement.x / World.Instance.WorldScale, Vector3.right) * Rotation;
			Rotation = Quaternion.AngleAxis(displacement.y / World.Instance.WorldScale, Vector3.up) * Rotation;
		}
	}

	public void SetRotation(Quaternion rotation) {
		Rotation = rotation;
	}

	public Quaternion GetNewRotation(Vector2 displacement)
    {
		Vector3 upAxis = transform.position - World.Instance.gameObject.transform.position;
        Vector3 leftAxis = Vector3.Cross(ForwardReference.transform.forward, upAxis);
        Vector3 forwardAxis = Vector3.Cross(upAxis, leftAxis);
		var tmpRotation = Rotation;
		tmpRotation = Quaternion.AngleAxis(-displacement.x / World.Instance.WorldScale, leftAxis) * tmpRotation;
		tmpRotation = Quaternion.AngleAxis(displacement.y / World.Instance.WorldScale, forwardAxis) * tmpRotation;
		return tmpRotation;
    }

	public Vector3 GetForwardAxis() {
		Vector3 upAxis = transform.position - World.Instance.gameObject.transform.position;
        Vector3 leftAxis = Vector3.Cross(ForwardReference.transform.forward, upAxis);
        return Vector3.Cross(upAxis, leftAxis);
	}

	public Vector3 GetUpAxis()
    {
		return transform.position - World.Instance.gameObject.transform.position;
    }

}
