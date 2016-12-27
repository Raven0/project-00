using UnityEngine;
using System.Collections;


public class ArrowKeysPlayerController : MonoBehaviour {
    public float moveSpeed;

    //Constrain movement to walkzone
    public Collider2D walkZone;
    private Vector2 minWalkPoint;
    private Vector2 maxWalkPoint;
    private bool hasWalkZone;

    //Jumping variables
    public float acceleration;//basically gravity
    public float jumpSpeed;//initial velocity
    public float jumpTime;//determines how long the jump will last
    private bool isJumping;
    private float landingY;//where character will land
    private float currTime;

    private bool isGrounded;
    private bool canMove;
    //private BoxCollider2D collider;

    //Shadow variables
    public GameObject shadow;

    private Animator anim;
    private Rigidbody2D myRigidBody;

    private bool playerIsMoving;
    private bool isMovingLeft;
    private bool isMovingRight;
    private bool isMovingUp;
    private bool isMovingDown;
    public Vector2 lastMove;

    private static bool playerExists;

    //Variables for attacking
    private static bool isAttacking;
    public float attackTime;
    private float attackTimeCounter;

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

        currTime = 0;
        isGrounded = true;
        landingY = transform.position.y;
        //collider = gameObject.GetComponent<BoxCollider2D>();

        if(walkZone != null)
        {
            minWalkPoint = walkZone.bounds.min;
            maxWalkPoint = walkZone.bounds.max;
            hasWalkZone = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        playerIsMoving = false;
        isMovingLeft = false;
        isMovingRight = false;
        isMovingUp = false;
        isMovingDown = false;

        if (isGrounded && isAttacking)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }

        //If we aren't attacking and not on the ground, we can move
        if (canMove)
            Movement();
        //Attack lag
        if (isAttacking)
        {
            if (attackTimeCounter > 0f)
            {
                attackTimeCounter -= Time.deltaTime;
            }
            else
            {
                isAttacking = false;
                anim.SetBool("PlayerIsAttacking", false);
            }
        }

        if (!isJumping)
        {
            landingY = transform.position.y;
        }

        //Put shadow in right place
        if (isJumping)
            shadow.transform.position = new Vector3(transform.position.x, landingY - 0.5f, transform.position.z);
        else
            shadow.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        //Update walkZone boundaries
        if (walkZone != null)
        {
            minWalkPoint = walkZone.bounds.min;
            maxWalkPoint = walkZone.bounds.max;
            hasWalkZone = true;
        }

        //Animation();
    }

    //Handles all of the characters movements, user inputs
    void Movement()
    {
        //Up
        if(Input.GetKey(KeyCode.UpArrow))
        {
            //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime, 0f, 0f));--before rigidbody update
            //myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, moveSpeed);

            if (hasWalkZone && transform.position.y > maxWalkPoint.y)
            {
                //if will move outside of walkzone, do nothing
            }
            else
            {
                transform.Translate(new Vector3(0f, moveSpeed * Time.deltaTime, 0f));
                isMovingUp = true;
                playerIsMoving = true;
                //lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);

                landingY += moveSpeed * Time.deltaTime;
            }
        }
        //Left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime, 0f, 0f));--before rigidbody update
            //myRigidBody.velocity = new Vector2(-moveSpeed, myRigidBody.velocity.y);
            if (hasWalkZone && transform.position.x < minWalkPoint.x)
            {
                //if will move outside of walkzone, do nothing
            }
            else
            {
                transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0f, 0f));
                isMovingLeft = true;
                playerIsMoving = true;
                //lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            }
        }
        //Down
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime, 0f, 0f));--before rigidbody update
            //myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, -moveSpeed);
            if (hasWalkZone && transform.position.y < minWalkPoint.y)
            {
                //if will move outside of walkzone, do nothing
            }
            else
            {
                transform.Translate(new Vector3(0f, -moveSpeed * Time.deltaTime, 0f));
                isMovingDown = true;
                playerIsMoving = true;
                //lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);

                landingY -= moveSpeed * Time.deltaTime;
            }
        }
        //Right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime, 0f, 0f));--before rigidbody update
            //myRigidBody.velocity = new Vector2(moveSpeed, myRigidBody.velocity.y);
            if (hasWalkZone && transform.position.x > maxWalkPoint.x)
            {
                //if will move outside of walkzone, do nothing
            }
            else
            {
                transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0f, 0f));
                isMovingRight = true;
                playerIsMoving = true;
                //lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
            }
        }
        if(!playerIsMoving)//No movement input
        {
            myRigidBody.velocity = Vector2.zero;
        }

        //Attack(set as "/")
        if (Input.GetKey(KeyCode.Slash))//GetKeyDown() only takes in the first instance the key is pressed
        {
            attackTimeCounter = attackTime;
            isAttacking = true;
            myRigidBody.velocity = Vector2.zero;//stop players movement while attacking
            anim.SetBool("PlayerIsAttacking", true);
        }

        //Jump(set as .)
        if (Input.GetKeyDown(KeyCode.Period))
        {
            isJumping = true;
            isGrounded = false;
            GetComponent<BoxCollider2D>().isTrigger = true;//Let this object pass over other objects while jumping
        }
        if (isJumping)
        {
            //Done jumping
            if (transform.position.y < landingY)//currTime >= jumpTime)
            {
                isJumping = false;
                isGrounded = true;
                GetComponent<BoxCollider2D>().isTrigger = false;

                currTime = 0;
                transform.position = new Vector3(transform.position.x, landingY, transform.position.z);
            }
            //Continue jump
            else
            {
                if (!isAttacking)
                {
                    float currVel = jumpSpeed - acceleration * currTime;
                    // float newY = jumpSpeed * currTime + (1 / 2) * acceleration * Mathf.Pow(currTime, 2) + landingY;
                    currTime += Time.deltaTime;
                    //float changeInY = jumpSpeed*currTime + (1 / 2) * acceleration * Mathf.Pow(currTime, 2);
                    //float currVel = jumpSpeed - acceleration * currTime;
                    transform.position = new Vector3(transform.position.x, transform.position.y + currVel, transform.position.z);
                }
            }
        }
    }

   /* //Handles animations
    void Animation()
    {
        anim.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
        anim.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));
        anim.SetBool("PlayerIsMoving", playerIsMoving);
        anim.SetFloat("LastMoveX", lastMove.x);
        anim.SetFloat("LastMoveY", lastMove.y);
    }*/
}
