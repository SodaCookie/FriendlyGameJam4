using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Animator animator;
	public MovementController controller;
	public float MouseSensitivity = 1f;
	public float ForwardSpeed = 1f;
	public float BackSpeed = 0.5f;
	public float RollForwardDistance = 2f;
	public float BackStepDistance = 3f;
	public float SideStepDistance = 5f;
	public float KneeSpeed = 20f;
	public bool Invincible = false;
	public GameObject Visual;
	private bool dashing = false;

	[Header("Combat Colliders")]
	public Collider LeftHandCollider; 
	public Collider RightHandCollider; 
	public Collider LeftLegCollider; 
	public Collider RightLegCollider;
	public List<Collider> HurtBoxs = new List<Collider>();
	public bool attacking = true;
	public List<Collider> onGoingAttacks = new List<Collider>(); 

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;

		// Load all keyframes and attack data
		AddEvent("kick_20", "Knee", 0.16f);
		AddEvent("punch_20", "Punch1", 0.01f);
		AddEvent("punch_21", "Punch2", 0.02f);
		AddEvent("punch_22", "Punch3", 0.05f);
		AddEvent("kick_21", "FlyKick", 0.01f);
		AddEvent("kick_22", "Kick1", 0.01f);
		AddEvent("kick_23", "Kick2", 0.01f);
		AddEvent("kick_24", "Kick3", 0.01f);
		AddEvent("kick_25", "Roundhouse", 0.01f);
		AddEvent("special_20_p", "UpperCut", 0.05f);

	}

	private void AddEvent(string clipName, string functionName, float time) {
		AnimationClip foundClip = null;
		foreach (var tmpClip in animator.runtimeAnimatorController.animationClips) {
			if (tmpClip.name == clipName) {
				foundClip = tmpClip;
				break;
			}
		}
		if (foundClip != null) {
			AnimationEvent animationEvent = new AnimationEvent();
            animationEvent.functionName = functionName;
			animationEvent.floatParameter = foundClip.length - time;
            animationEvent.time = time;
			foundClip.AddEvent(animationEvent);
		}
		else {
			Debug.LogError("Clip not found");
		}
	}

	public void Hurt(float knockBack, Vector3 rotationAxis) {
		Invincible = true;
		foreach (var coll in HurtBoxs)
        {
            coll.enabled = false;
        }
		World.Instance.PlayerController.GetComponent<PlayerController>().animator.SetTrigger("Hurt");
		StartCoroutine(World.Instance.PlayerController.GetComponent<PlayerController>().KnockBack(knockBack, rotationAxis, 0.4f));
        StartCoroutine(World.Instance.PlayerController.GetComponent<PlayerController>().InvincibleFrames(1.5f));
	}

	public void ReallyHurt(float knockBack, Vector3 rotationAxis) {
		Invincible = true;
		foreach (var coll in HurtBoxs)
        {
            coll.enabled = false;
        }
		World.Instance.PlayerController.GetComponent<PlayerController>().animator.SetTrigger("ReallyHurt");
		StartCoroutine(World.Instance.PlayerController.GetComponent<PlayerController>().KnockBack(knockBack, rotationAxis, 0.5f));
        StartCoroutine(World.Instance.PlayerController.GetComponent<PlayerController>().InvincibleFrames(2.5f));
	}

	public void Knee(float remainingTime) {
		StartCoroutine(Attack("Knee", 4, 10, 18, RightLegCollider, remainingTime, true));
	}

	public void FlyKick(float remainingTime)
    {
		StartCoroutine(Attack("Kick", 3, 6, 12, RightLegCollider, 0.4f, true));
    }

	public void UpperCut(float remainingTime)
    {
        StartCoroutine(Attack("Upper Cut", 0, 8, 12, RightHandCollider, 0, true));
		StartCoroutine(InvincibleFramesNoFlash(remainingTime));
    }

	public void Roundhouse(float remainingTime)
    {
		StartCoroutine(Attack("Roundhouse", 5, 4, 15, RightLegCollider, 0.2f, true));
		StartCoroutine(InvincibleFramesNoFlash(remainingTime));
    }

	public void Punch1(float remainingTime)
    {
        StartCoroutine(Attack("Basic Strike 1", 0, 1, 1, LeftHandCollider, 0));
		StartCoroutine(InvincibleFramesNoFlash(remainingTime));
    }

	public void Punch2(float remainingTime)
    {
		StartCoroutine(Attack("Basic Strike 2", 1, 2, 2, RightHandCollider, 0));
    }

	public void Punch3(float remainingTime)
    {
		StartCoroutine(Attack("Basic Strike 3", 5, 3, 4, RightHandCollider, 0, true));
		StartCoroutine(InvincibleFramesNoFlash(remainingTime));
    }

	public void Kick1(float remainingTime)
    {
		StartCoroutine(Attack("Basic Kick 1", 0.1f, 2, 2, RightLegCollider, 0));
		StartCoroutine(InvincibleFramesNoFlash(remainingTime));
    }
    
	public void Kick2(float remainingTime)
    {
		StartCoroutine(Attack("Basic Kick 2", 0.1f, 4, 4, RightLegCollider, 0));
    }

	public void Kick3(float remainingTime)
    {
		StartCoroutine(Attack("Basic Kick 3", 0.1f, 6, 6, RightLegCollider, 0));
    }

	private void Update()
	{
		if (PlayerValues.Health <= 0) {
			animator.SetBool("Death", true);
			animator.SetBool("Finished", true);
			foreach (var coll in HurtBoxs)
            {
                coll.enabled = false;
            }
			Invincible = true;
			Visual.SetActive(true);
			StopAllCoroutines();
			enabled = false;
		}
		else {
			HandleMovement();
			HandleRotation();
		}
	}

	private void ResetTriggers() {
		animator.ResetTrigger("Dodge");
		animator.ResetTrigger("Attack");
		animator.ResetTrigger("Kick");
	}

	void HandleRotation() {
		Vector3 upAxis = controller.GetUpAxis();
		transform.Rotate(upAxis, Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime, Space.World);
	}
    
	IEnumerator Attack(string stateName, float speed, float damage, float knockback, Collider coll, float time, bool resetTriggers=false)
    {
		onGoingAttacks.Add(coll);
		var next = animator.GetNextAnimatorStateInfo(0);
        coll.enabled = true;
        coll.GetComponent<Hitbox>().Damage = damage;
        coll.GetComponent<Hitbox>().Knockback = knockback;

        float start = Time.time;
        while (next.shortNameHash == Animator.StringToHash(stateName) || Time.time - start < time)
        {
            controller.Move(new Vector2(speed * Time.deltaTime, 0));
            next = animator.GetNextAnimatorStateInfo(0);
            yield return null;
        }
		onGoingAttacks.Remove(coll);
		if (!onGoingAttacks.Contains(coll)) {
			coll.enabled = false;
		}
		if (resetTriggers) {
			ResetTriggers();
		}
    }
    
	public IEnumerator InvincibleFramesNoFlash(float duration)
    {
		foreach (var coll in HurtBoxs)
        {
            coll.enabled = false;
        }
        Invincible = true;
		yield return new WaitForSeconds(duration);
        foreach (var coll in HurtBoxs)
        {
            coll.enabled = true;
        }
        Invincible = false;
    }

	public IEnumerator InvincibleFrames(float duration)
	{
		float start = Time.time;
		while (Time.time - start < duration) {
			Visual.SetActive(!Visual.activeSelf);
			yield return new WaitForSeconds(0.03f);
		}
		Visual.SetActive(true);
		foreach (var coll in HurtBoxs)
        {
            coll.enabled = true;
        }
		Invincible = false;
	}

	public IEnumerator KnockBack(float knockback, Vector3 axis, float duration)
	{
        float start = Time.time;
		while (Time.time - start < duration)
        {
			controller.Rotation = Quaternion.AngleAxis(knockback * Time.deltaTime / World.Instance.WorldScale, axis) * controller.Rotation;
            yield return null;
        } 
	}

	IEnumerator Dash(Quaternion curRot, Quaternion targetRot, string stateName, float height=0f, float startClamp=0f, float endClamp=0f) {
		dashing = true;
		var current = animator.GetNextAnimatorStateInfo(0);
		yield return new WaitUntil(() =>
		{
			current = animator.GetCurrentAnimatorStateInfo(0);
			return current.shortNameHash == Animator.StringToHash(stateName);
		});
		foreach (var coll in HurtBoxs) {
			coll.enabled = false;
		}

		float maxValue = 0;
		while (current.shortNameHash == Animator.StringToHash(stateName)) {
			current = animator.GetCurrentAnimatorStateInfo(0);
			if (current.normalizedTime > maxValue) {
				float t;
				t = current.normalizedTime;
				controller.SetRotation(Quaternion.Slerp(curRot, targetRot, Mathf.Clamp(current.normalizedTime, startClamp, 1 - endClamp)));
				controller.Height = Mathf.Clamp((-Mathf.Pow(2 * current.normalizedTime - 1, 2) + 1) * height, 0, float.PositiveInfinity);
				maxValue = current.normalizedTime;
				animator.ResetTrigger("Dodge");
			}
			yield return null;
		}
		controller.Height = 0;

		foreach (var coll in HurtBoxs)
        {
			coll.enabled = true;
        }
		dashing = false;
	}

	void HandleMovement()
	{
		float speed = 0;

		// Handle Key Downs
		if (Input.GetKey(KeyCode.W))
		{
			speed = ForwardSpeed;
			animator.SetFloat("Forward", 1);
		}
		if (Input.GetKey(KeyCode.S))
		{
			speed = -BackSpeed;
			animator.SetFloat("Forward", -1);
		}
		if (Input.GetKey(KeyCode.A))
		{
			animator.SetBool("Left", true);
		}
		else {
			animator.SetBool("Left", false);
		}
		if (Input.GetKey(KeyCode.D))
		{
			animator.SetBool("Right", true);
		}
		else {
			animator.SetBool("Right", false);

		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			animator.SetTrigger("Dodge");
		}
		if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
		if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Kick");
        }

		if (Mathf.Approximately(speed, 0)) {
			animator.SetBool("Moving", false);
			animator.SetFloat("Forward", 0);
		}
		else {
			animator.SetBool("Moving", true);
		}

		// Handle movement
		var current = animator.GetCurrentAnimatorStateInfo(0);
		var next = animator.GetNextAnimatorStateInfo(0);
		if (current.shortNameHash == Animator.StringToHash("Moving"))
		{
			controller.Move(new Vector2(speed, 0) * Time.deltaTime);
		}

		// Start a roll
		if (next.shortNameHash == Animator.StringToHash("Roll Forward") && !dashing) {
			StartCoroutine(Dash(controller.Rotation, controller.GetNewRotation(new Vector2(RollForwardDistance, 0)), "Roll Forward", 0, 0, 0f));
			ResetTriggers();
		}
        
        // Start a backjump
		if (next.shortNameHash == Animator.StringToHash("Backjump") && !dashing)
        {
			StartCoroutine(Dash(controller.Rotation, controller.GetNewRotation(new Vector2(-BackStepDistance, 0)), "Backjump", 0f, 0, 0.3f));
			ResetTriggers();
        }

		// Start a leftjump
		if (next.shortNameHash == Animator.StringToHash("LeftJump") && !dashing)
        {
			StartCoroutine(Dash(controller.Rotation, controller.GetNewRotation(new Vector2(speed/2, SideStepDistance)), "LeftJump", 0f, 0, 0.3f));
			ResetTriggers();
        }

		// Start a rightjump
		if (next.shortNameHash == Animator.StringToHash("RightJump") && !dashing)
        {
			StartCoroutine(Dash(controller.Rotation, controller.GetNewRotation(new Vector2(speed/2, -SideStepDistance)), "RightJump", 0f, 0, 0.3f));
			ResetTriggers();
        }
	}
}
