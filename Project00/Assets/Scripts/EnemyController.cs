using UnityEngine;
using System.Collections;

/*Follows trackingTarget relentlessly, attacking when in range*/
public class EnemyController : MonoBehaviour {

	public GameObject trackingTarget;
	public float moveSpeed;
	private Animator anim;
	private Rigidbody2D myRigidBody;

	//variables for tracking
	private float euclideanDist;
	private Vector2 targetPosition;
	private Vector2 ourPosition;
	public float attackRange;
	private float xDist;
	private float yDist;
	//variables for attacking animation
	private bool isAttacking;
	public float attackTime;
	private float attackTimeCounter;

	//variables for knockback animation
	private bool isKnockedBack;//lets the object that hit us set this
	public float knockbackTime;
	private float knockbackTimeCounter;
	private float flinchX;//-1 if we need to flinch left, 1 if right
	public void setFlinchX(float val){
		flinchX = val;
	}
	public void	setKnockBack(bool val){
		//lets other classes set isKnockedback
		isKnockedBack = val;
	}

	//variables for animation
	private bool playerIsMoving;
	public Vector2 lastMove;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		myRigidBody = GetComponent<Rigidbody2D>();

		isAttacking = false;
		isKnockedBack = false;

		knockbackTimeCounter = -1;
	}
	
	// Update is called once per frame
	void Update () {
		playerIsMoving = false;

		if (isKnockedBack) {
			if (knockbackTimeCounter == -1) {
				//				Debug.Log ("Entered flinch block");
				anim.SetBool ("Is_Knocked_Back", true);
				knockbackTimeCounter = knockbackTime;
				anim.SetFloat ("Flinch_X", flinchX);
			}

			//			Debug.Log ("knockBackCounter: " + knockbackTimeCounter.ToString ());
			if (knockbackTimeCounter > 0f) {
				knockbackTimeCounter -= Time.deltaTime;
			} else {
				isKnockedBack = false;
				myRigidBody.velocity = Vector2.zero;
				knockbackTimeCounter = -1f;

				//set anim variable to false
				anim.SetBool ("Is_Knocked_Back", false);
			}
			return;

		}
		else if (isAttacking) {
			//attack lag
			if (attackTimeCounter > 0f)
			{
				attackTimeCounter -= Time.deltaTime;
			}
			else
			{
				isAttacking = false;
				anim.SetBool("Attack_1", false);
			}
		}
		else{
			//calculate dist from trackingTarget
			targetPosition = trackingTarget.transform.position;
			ourPosition = transform.position;

			xDist = targetPosition.x - ourPosition.x;//(-) if we need to go left, (+) if we need to go right
			yDist = targetPosition.y - ourPosition.y;

			euclideanDist = Mathf.Sqrt (Mathf.Pow(xDist, 2) + Mathf.Pow(yDist, 2));
//			Debug.Log ("euclideanDist: " + euclideanDist.ToString ("0.0######"));


			//if we are close enough, attack
			if (euclideanDist <= attackRange) {
				isAttacking = true;
				//stop moving
				myRigidBody.velocity = Vector2.zero;
				anim.SetBool ("Attack_1", true);
				attackTimeCounter = attackTime;
			}
			//else, set velocity to move towards trackingTarget's position
			else {
				//scale xDist, yDist by moveSpeed/euclideanDist to get a net force of moveSpeed in the right direction
				myRigidBody.velocity = new Vector2(xDist*(moveSpeed/euclideanDist), yDist*(moveSpeed/euclideanDist));
				playerIsMoving = true;

			}
		}

		Animation ();
	}

	//Handles animations
	void Animation()
	{
		anim.SetFloat("Move_X", xDist);
		anim.SetFloat("Move_Y", yDist);
		anim.SetBool("Player_Is_Moving", playerIsMoving);
		anim.SetFloat("Last_Move_X", xDist);
		anim.SetFloat("Last_Move_Y", yDist);
	}
}
