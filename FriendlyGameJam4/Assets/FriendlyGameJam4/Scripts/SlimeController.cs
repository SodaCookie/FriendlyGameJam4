using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
	public Animator animator;
	public AnimationClip clip;
	private EnemyController enemyController;

	public float JumpRange = 10f;
	public float AttackCooldown = 2f;
	public bool Attacking = false;
	public float JumpHeight = 3f;
	public float JumpTime = 1f;

	private void Start()
	{
		animator.SetFloat("jump", 1 / clip.length / JumpTime);
		enemyController = GetComponent<EnemyController>();
	}

	private IEnumerator JumpTowards()
	{
		Quaternion previous = enemyController.thisController.Rotation;
		Quaternion target = enemyController.playerController.Rotation;
		Attacking = true;
		float start = Time.time;
		while (Time.time - start < JumpTime)
		{
			float t = (Time.time - start) / JumpTime;
			enemyController.thisController.Height = Mathf.Clamp((-Mathf.Pow(2 * t - 1, 2) + 1) * JumpHeight, 0, float.PositiveInfinity);
			enemyController.thisController.Rotation = Quaternion.Slerp(previous, target, t);
			yield return null;
		}
		enemyController.thisController.Height = 0;
		Attacking = false;
	}

	private void LateUpdate()
	{
		var current = animator.GetCurrentAnimatorStateInfo(0);
        if (current.shortNameHash == Animator.StringToHash("Attack"))
        {
			StartCoroutine(enemyController.PutOnCooldown(AttackCooldown));
            StartCoroutine(JumpTowards());
        }

	}
}
