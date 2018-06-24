using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour {

	public MovementController controller;
	public GameObject HitEffect;
	public float height = 0.5f;
	public Vector3 axis;
	public float speed;

	// Use this for initialization
	void Start () {
		Invoke("Remove", 10f);
		controller.Height = height;
	}

	void Remove() {
		Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Hurtbox") && collision.collider.tag == "Player" && !World.Instance.PlayerController.GetComponent<PlayerController>().Invincible)
		{
			Vector3 avgPoint = new Vector3();
            Collider thisCollider = null;
            foreach (var contact in collision.contacts)
            {
                avgPoint += contact.point;
                thisCollider = contact.thisCollider;
            }

            avgPoint /= collision.contacts.Length;
            var hitbox = thisCollider.GetComponent<Hitbox>();
            Vector3 axis = Vector3.Cross(Camera.main.transform.forward, controller.GetUpAxis());
            var go = Instantiate(HitEffect, avgPoint, new Quaternion());
            go.transform.localScale *= Mathf.Clamp(hitbox.Damage, 2, float.PositiveInfinity);

            World.Instance.PlayerController.StopAllCoroutines();
            PlayerValues.Health -= (int)hitbox.Damage;
            if (hitbox.Damage > 4)
            {
                World.Instance.PlayerController.GetComponent<PlayerController>().ReallyHurt(hitbox.Knockback, axis);
            }
            else
            {
                World.Instance.PlayerController.GetComponent<PlayerController>().Hurt(hitbox.Knockback, axis);
            }
			Destroy(gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		Quaternion prev = controller.Rotation;
		controller.Rotation = Quaternion.AngleAxis(speed * Time.deltaTime, axis) * controller.Rotation;
	}
}
