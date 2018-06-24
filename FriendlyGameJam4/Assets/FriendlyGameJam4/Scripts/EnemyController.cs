using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour 
{
	[Header("References")]
	public Animator animator;
    public MovementController playerController;
    public MovementController thisController;
	public GameObject HitEffect;
	public GameObject HealthPrefab;

    [Header("Attack Attributes")]
	public bool CanAttack = true;
	public float AggroRange = 20f;
	public float AttackRange = 10f;
    public float HoverRange = 5f;
    public float Speed = 10f;
	public float Weight = 1f;
	public int Health = 10;
	public float HealthDropChance = 0.02f;
	public List<string> AggroState = new List<string>();
	private List<int> aggroStateTokens = new List<int>();
    
	private void Start()
	{
		playerController = World.Instance.PlayerController;
        thisController = GetComponent<MovementController>();
		animator.Update(Random.value);
		foreach (string state in AggroState) {
			aggroStateTokens.Add(Animator.StringToHash(state));
		}
	}
    
	private IEnumerator KnockBack(float force, Vector3 rotationAxis)
    {
        Quaternion previous = thisController.Rotation;
        Quaternion target = playerController.Rotation;
        float start = Time.time - 0.001f;
		float weightHeight = force / 5 * Mathf.Clamp(Weight - force, 1, float.PositiveInfinity);
        float previousHeight = thisController.Height;
        do
        {
            thisController.Height = (-Mathf.Pow(2 * (Time.time - start) - 1, 2) + 1) * weightHeight + previousHeight;
            thisController.Rotation = Quaternion.AngleAxis(force * 2 / (Weight) * Time.deltaTime, rotationAxis) * thisController.Rotation;
            animator.SetBool("hurt", true);
			yield return null;
        } while (thisController.Height > 0);
        thisController.Height = 0;
        animator.SetBool("hurt", false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Hitbox") && collision.collider.tag == "Player")
        {
            Vector3 avgPoint = new Vector3();
            foreach (var contact in collision.contacts)
            {
                avgPoint += contact.point;
            }

            avgPoint /= collision.contacts.Length;
            var hitbox = collision.collider.GetComponent<Hitbox>();
            Vector3 axis = Vector3.Cross(thisController.GetUpAxis(), Camera.main.transform.forward);
			var go = Instantiate(HitEffect, avgPoint, new Quaternion());
			Health -= (int)Mathf.Round(hitbox.Damage);
			if (Health < 0) {
				go.transform.localScale *= Mathf.Clamp(hitbox.Damage, 4, float.PositiveInfinity);
				Camera.main.GetComponent<CameraShake>().shakeDuration = 0.1f;
                Camera.main.GetComponent<CameraShake>().shakeAmount = hitbox.Damage / 10;
                Camera.main.GetComponent<CameraShake>().enabled = true;
				if (hitbox.Damage / 10 > 0.6f)
                {
                    StartCoroutine(TimeStop(0.1f));
                }
			} else {
				go.transform.localScale *= Mathf.Clamp(hitbox.Damage, 2, float.PositiveInfinity);
                if (hitbox.Damage / 10 > 0.3f * Weight)
                {
                    Camera.main.GetComponent<CameraShake>().shakeDuration = 0.1f;
                    Camera.main.GetComponent<CameraShake>().shakeAmount = hitbox.Damage / 10;
                    Camera.main.GetComponent<CameraShake>().enabled = true;
                    if (hitbox.Damage / 10 > 0.6f)
                    {
                        StartCoroutine(TimeStop(0.2f));
                    }
                }
			}
            
            StopAllCoroutines();
            if (!CanAttack) {
                CanAttack = true;
            }
            StartCoroutine(KnockBack(hitbox.Knockback, axis));
        }
		else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Hurtbox") && collision.collider.tag == "Player" && !World.Instance.PlayerController.GetComponent<PlayerController>().Invincible) {
			Vector3 avgPoint = new Vector3();
			Collider thisCollider = null;
            foreach (var contact in collision.contacts)
            {
                avgPoint += contact.point;
				thisCollider = contact.thisCollider;
            }

            avgPoint /= collision.contacts.Length;
			var hitbox = thisCollider.GetComponent<Hitbox>();
			Vector3 axis = Vector3.Cross(Camera.main.transform.forward, thisController.GetUpAxis());
            var go = Instantiate(HitEffect, avgPoint, new Quaternion());
            go.transform.localScale *= Mathf.Clamp(hitbox.Damage, 2, float.PositiveInfinity);

            World.Instance.PlayerController.StopAllCoroutines();
			PlayerValues.Health -= (int)hitbox.Damage;
			if (hitbox.Damage > 4) {
				World.Instance.PlayerController.GetComponent<PlayerController>().ReallyHurt(hitbox.Knockback, axis);
			}
			else {
				World.Instance.PlayerController.GetComponent<PlayerController>().Hurt(hitbox.Knockback, axis);
			}
		}
    }

	IEnumerator TimeStop(float duration) {
		float previousTimeScale = Time.timeScale;
		Time.timeScale = 1f;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = previousTimeScale;
	}

	// Update is called once per frame
    void Update()
    {
		if (Vector3.Angle(playerController.Rotation * Vector3.up, thisController.Rotation * Vector3.up) < AggroRange / World.Instance.WorldScale)
        {
            animator.SetBool("aggro", true);
			if (Vector3.Angle(playerController.Rotation * Vector3.up, thisController.Rotation * Vector3.up) > HoverRange / World.Instance.WorldScale)
            {
                thisController.Rotation = Quaternion.Slerp(thisController.Rotation, playerController.Rotation, Speed * Time.deltaTime);
		}
        }
        else
        {
            animator.SetBool("aggro", false);
        }
		var current = animator.GetCurrentAnimatorStateInfo(0);
		if (aggroStateTokens.Contains(current.shortNameHash) && CanAttack && Vector3.Angle(thisController.Rotation * Vector3.up, playerController.Rotation * Vector3.up) < AttackRange / World.Instance.WorldScale)
        {
            animator.SetTrigger("attack");
        }
		if (Health <= 0) {
			if (Random.value < HealthDropChance) {
				var go = Instantiate(HealthPrefab);
				go.GetComponent<MovementController>().Rotation = thisController.Rotation;
				go.GetComponent<MovementController>().UpdatePosition();
			}
			Destroy(gameObject);
		}
    }

	public IEnumerator PutOnCooldown(float time)
    {
        CanAttack = false;
		yield return new WaitForSeconds(time);
        CanAttack = true;
    }
}
