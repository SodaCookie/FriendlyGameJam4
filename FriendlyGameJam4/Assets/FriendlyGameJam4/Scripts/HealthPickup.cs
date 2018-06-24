using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

	public float Height;
	public GameObject HitEffect;
	public int Amount;

	private void Start()
	{
		GetComponent<MovementController>().Height = Height;
	}

	private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
			Vector3 avgPoint = new Vector3();
            foreach (var contact in collision.contacts)
            {
                avgPoint += contact.point;
            }

            avgPoint /= collision.contacts.Length;
            var hitbox = collision.collider.GetComponent<Hitbox>();
			Vector3 axis = Vector3.Cross(GetComponent<MovementController>().GetUpAxis(), Camera.main.transform.forward);
            var go = Instantiate(HitEffect, avgPoint, new Quaternion());
			go.transform.localScale *= 5;
			Camera.main.GetComponent<CameraShake>().shakeDuration = 0.1f;
            Camera.main.GetComponent<CameraShake>().shakeAmount = 0.7f;
            Camera.main.GetComponent<CameraShake>().enabled = true;
			PlayerValues.Health = Mathf.Clamp(PlayerValues.Health + Amount, 0, 20);
			Destroy(gameObject);
        }
    }
}
