using UnityEngine;
using System.Collections;

//WASD , J-attack
public class WASDPlayerController : MonoBehaviour {
    public float moveSpeed;

    //Constrain movement to walkzone
    public Collider2D walkZone;
    private Vector2 minWalkPoint;
    private Vector2 maxWalkPoint;
    private bool hasWalkZone;


    private bool isGrounded;
    private bool canMove;

    private Animator anim;
    private Rigidbody2D myRigidBody;

    private bool playerIsMoving;
    public Vector2 lastMove;

    private static bool playerExists;

    //Variables for attacking
    private static bool isAttacking;
    public float attackTime;
    private float attackTimeCounter;
	//Variables for rolling
	public float rollSpeed;
	private static  bool isRolling;
	public float rollTime;
	private float rollTimeCounter;

	//variables for knockback animation
	private bool isKnockedBack;//lets the objects that hit us alter this
	public float knockbackTime;
	private float knockbackTimeCounter;
	private float flinchX;//-1 if we need to flinch left, 1 if right
	public void setFlinchX(float val){
		flinchX = val;
	}

	public void	setKnockBack(bool val){
		//lets other classes set isKnockedback
		isKnockedBack = val;
//		Debug.Log ("WASDPlayerController's isKnockedBack set to " + val.ToString());
	}

	public bool isInvulnerable;//let the thing attacking us check this value
	public float rollIFrames;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();

        /* //Deals with duplicates of the player when any scene loading happens
         if (!playerExists)
         {
             playerExists = true;
             DontDestroyOnLoad(transform.gameObject);
         }
         else
         {
             Destroy(gameObject);
         }*/

        isGrounded = true;
		isAttacking = false;
		isKnockedBack = false;
		isInvulnerable = false;

		knockbackTimeCounter = -1;

		//start off facing right
		lastMove = new Vector2(1f, 0f);

        //Update walkZone boundaries
        if (walkZone != null)
        {
            minWalkPoint = walkZone.bounds.min;
            maxWalkPoint = walkZone.bounds.max;
            hasWalkZone = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        playerIsMoving = false;

//		Debug.Log ("isKnockedBack: " + isKnockedBack.ToString ());

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
		//Rolling
		else if (isRolling) {
			if (isInvulnerable && (rollTime - rollTimeCounter) >= rollIFrames) {
				//ran out of iframes
				isInvulnerable = false;
			}

			if (rollTimeCounter > 0f) {
				rollTimeCounter -= Time.deltaTime;
			}
			else
			{
				isRolling = false;
				anim.SetBool("Rolling", false);
				myRigidBody.velocity = Vector2.zero;//stop moving
				//reset boxcollider
				GetComponent<BoxCollider2D>().isTrigger = false;

			}
		}
        //Attack lag
		else if(isAttacking)
         {
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
		//If we aren't attacking or rolling or getting knocked back, we can move
		else
			Movement();

        //Update walkZone boundaries
        if (walkZone != null)
        {
            minWalkPoint = walkZone.bounds.min;
            maxWalkPoint = walkZone.bounds.max;
            hasWalkZone = true;
        }

        Animation();
    }

    //Handles all of the characters movements, user inputs
    void Movement()
    {
        //Up
        if (Input.GetKey(KeyCode.W))
        {
			if (!hasWalkZone || (hasWalkZone && transform.position.y <= maxWalkPoint.y))
            {
                transform.Translate(new Vector3(0f, moveSpeed * Time.deltaTime, 0f));
//                isMovingUp = true;
                playerIsMoving = true;
                lastMove = new Vector2(0f, Input.GetAxisRaw("Vertical"));
			}
        }
        //Left
        if (Input.GetKey(KeyCode.A))
        {

			if (!hasWalkZone || (hasWalkZone && transform.position.x >= minWalkPoint.x))
            {
                transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0f, 0f));
//                isMovingLeft = true;
                playerIsMoving = true;
                lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            }
        }
        //Down
        if (Input.GetKey(KeyCode.S))
        {
			if (!hasWalkZone || (hasWalkZone && transform.position.y >= minWalkPoint.y))
            {
                transform.Translate(new Vector3(0f, -moveSpeed * Time.deltaTime, 0f));
//                isMovingDown = true;
                playerIsMoving = true;
                lastMove = new Vector2(0f, Input.GetAxisRaw("Vertical"));
            }
        }
        //Right
        if (Input.GetKey(KeyCode.D))
        {
			if (!hasWalkZone || (hasWalkZone && transform.position.x <= maxWalkPoint.x)){
                transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0f, 0f));
//                isMovingRight = true;
                playerIsMoving = true;
                lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            }
        }
        if (!playerIsMoving)//No movement input
        {
            myRigidBody.velocity = Vector2.zero;
        }

        //Attack(set as J)
		if (Input.GetKeyDown(KeyCode.J))//GetKeyDown() only takes in the first instance the key is pressed
        {
            attackTimeCounter = attackTime;
            isAttacking = true;
            myRigidBody.velocity = Vector2.zero;//stop players movement while attacking
            anim.SetBool("Attack_1", true);

			float attack_step_distance = 0.5f;
			//Move a bit towards direction we're facing
			transform.Translate(new Vector3(lastMove.x*(attack_step_distance), lastMove.y*(attack_step_distance), 0f));
        }

		//Roll(set as Spacebar)
		if (Input.GetKeyDown (KeyCode.Space)) {
			rollTimeCounter = rollTime;
			isRolling = true;
			anim.SetBool ("Rolling", true);
			//Roll towards direction we're facing
			myRigidBody.velocity = new Vector2(rollSpeed*lastMove.x, rollSpeed*lastMove.y);

			//Deactivate boxcollider, so we can roll past things
			GetComponent<BoxCollider2D>().isTrigger = true;
			//Set it so we are invincible
			isInvulnerable = true;
		}
    }

    //Handles animations
    void Animation()
    {
        anim.SetFloat("Move_X", Input.GetAxisRaw("Horizontal"));
        anim.SetFloat("Move_Y", Input.GetAxisRaw("Vertical"));
		anim.SetBool("Player_Is_Moving", playerIsMoving);
        anim.SetFloat("Last_Move_X", lastMove.x);
        anim.SetFloat("Last_Move_Y", lastMove.y);
    }
}
