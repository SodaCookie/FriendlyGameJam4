using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
	N, NE, E, SE, S, SW, W, NW
} 

public class ShooterController : MonoBehaviour {

	public Animator animator;
	public float DirectionChangeTime = 5f;
	public float AttackCooldown = 1f;
	public float MoveSpeed = 1f;
	public float BulletSpeed = 20f;
	public GameObject bullet;
    private EnemyController enemyController;
	private MovementController movementController;
	private Direction direction = Direction.N;

	private void Start()
	{
		InvokeRepeating("ChangeDirection", 0, DirectionChangeTime);
		movementController = GetComponent<MovementController>();
		enemyController = GetComponent<EnemyController>();
	}

	private IEnumerator FireForward() {
		yield return new WaitForSeconds(0.3f);
		Vector3 upAxis = movementController.GetUpAxis();
		Vector3 leftAxis = Vector3.Cross(upAxis, transform.position - Camera.main.transform.position);
		Vector3 rotationAxis = Vector3.Cross(upAxis, leftAxis);
		var go = Instantiate(bullet);
		go.GetComponent<MovementController>().Rotation = movementController.Rotation;
		go.GetComponent<MovementController>().UpdatePosition();
		go.GetComponent<BulletMovement>().axis = rotationAxis;
		go.GetComponent<BulletMovement>().speed = BulletSpeed;
	}

	private IEnumerator FireBack()
    {
        yield return new WaitForSeconds(0.3f);
        Vector3 upAxis = movementController.GetUpAxis();
        Vector3 leftAxis = Vector3.Cross(upAxis, transform.position - Camera.main.transform.position);
        Vector3 rotationAxis = Vector3.Cross(leftAxis, upAxis);
        var go = Instantiate(bullet);
        go.GetComponent<MovementController>().Rotation = movementController.Rotation;
        go.GetComponent<MovementController>().UpdatePosition();
		go.GetComponent<BulletMovement>().axis = rotationAxis;
		go.GetComponent<BulletMovement>().speed = BulletSpeed;
    }
    
	private void ChangeDirection() {
		direction = (Direction) Random.Range(0, 7);
		animator.SetBool("forward", !animator.GetBool("forward"));
	}

	private void LateUpdate()
    {
        var current = animator.GetCurrentAnimatorStateInfo(0);
        if (current.shortNameHash == Animator.StringToHash("Attack Forward") && enemyController.CanAttack)
        {
			StartCoroutine(FireForward());
			StartCoroutine(enemyController.PutOnCooldown(AttackCooldown));
        }
		if (current.shortNameHash == Animator.StringToHash("Attack Back") && enemyController.CanAttack)
        {
            StartCoroutine(FireBack());
            StartCoroutine(enemyController.PutOnCooldown(AttackCooldown));
        }

		switch (direction) {
			case Direction.N:
				movementController.Move(new Vector2(1, 0) * MoveSpeed * Time.deltaTime);
				break;
			case Direction.NE:
				movementController.Move(new Vector2(1, 1).normalized * MoveSpeed * Time.deltaTime);
                break;
			case Direction.E:
				movementController.Move(new Vector2(0, 1) * MoveSpeed * Time.deltaTime);
                break;
			case Direction.SE:
				movementController.Move(new Vector2(-1, 1).normalized * MoveSpeed * Time.deltaTime);
                break;
			case Direction.S:
				movementController.Move(new Vector2(-1, 0) * MoveSpeed * Time.deltaTime);
                break;
			case Direction.SW:
				movementController.Move(new Vector2(-1, -1).normalized * MoveSpeed * Time.deltaTime);
                break;
			case Direction.W:
				movementController.Move(new Vector2(0, -1) * MoveSpeed * Time.deltaTime);
                break;
			case Direction.NW:
				movementController.Move(new Vector2(1, -1).normalized * MoveSpeed * Time.deltaTime);
                break;
		}
    }
}
