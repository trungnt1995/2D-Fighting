using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public static PlayerController instance;
    
    public Rigidbody2D theRB;
    public float moveSpeed;
    public float runSpeed;

    public float jumpHeight;
    public float jumpDistance;
    public float baseTimeJump;
    public float jumpTimeCount = 0;
    public bool isAttack;

    public Animator myAnim;
    //public SpriteRenderer spriteRenderer;
    // public GameObject boxColliderStand;
    private float axisX, axisY;
    
    public float prevPosY;
    public float currentPosY;
    public bool isGround;
    private bool isDead = false;
    private UnitState playerState;
    public DIRECTION playerDir;

    float ButtonCooler  = .5f; // Half a second before reset
    int ButtonCount  = 0;

    //a list of states where movement can take place
	private List<UNITSTATE> MovementStates = new List<UNITSTATE> {
		UNITSTATE.IDLE,
		UNITSTATE.WALK,
		UNITSTATE.RUN,
		UNITSTATE.JUMPING,
		UNITSTATE.JUMPKICK,
		UNITSTATE.LAND,
		UNITSTATE.DEFEND,
	};

    // Start is called before the first frame update
    void Start()
    {   
        instance = this;
        myAnim =  GetComponent<Animator>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        theRB = GetComponent<Rigidbody2D>();
        prevPosY = gameObject.transform.position.y;
        playerState = GetComponent<UnitState>();
    }

    /// Update is called every frame, if the MonoBehaviour is enabled.

    
    void Update()
    {
        JumpPhysics();         
    }

    void FixedUpdate()
    {   
        if(!MovementStates.Contains(playerState.currentState) || isDead) 
        {
            theRB.velocity = Vector3.zero;
            return;
        }
        
        MoveOnGround();
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {   
           Jump();
        }
        onRun();
        Jumping();
        GetDirection();
    }

    public void Jump()
    {
        prevPosY = gameObject.transform.position.y;
        jumpTimeCount  = baseTimeJump;
    }

    public void Jumping()
    {
          if(jumpTimeCount > 0)
        {   
            jumpTimeCount  -= Time.deltaTime;
            if(transform.localScale.x == -1)
            {
                theRB.velocity = new Vector2(jumpDistance * -1, jumpHeight);
            }else
            {
                theRB.velocity = new Vector2(jumpDistance, jumpHeight);
            }    
        }
    }

    public void JumpPhysics()
    {
        currentPosY = gameObject.transform.position.y;
        if(jumpTimeCount > 0 && playerState.currentState != UNITSTATE.LAND)
        {   
            playerState.SetState(UNITSTATE.JUMPING);
            theRB.gravityScale = 5;
            isGround = false;
            myAnim.SetBool("Jump", true);
            // boxColliderStand.GetComponent<BoxCollider2D>().isTrigger = true;
        }else if(!isGround && prevPosY >= currentPosY)
        {   
            HasLand();      
        }
    }

    public void MoveOnGround()
    {   
        float CurrentSpeed = playerState.currentState == UNITSTATE.RUN ? runSpeed : moveSpeed;
        axisX = Input.GetAxis("Horizontal");
        axisY = Input.GetAxis("Vertical");
        if(isGround)
        {   
                theRB.velocity = new Vector2(axisX, axisY * .5f) * CurrentSpeed;
                if(axisX != 0 || axisY != 0 )
                {   
                    if(playerState.currentState == UNITSTATE.RUN)
                    {
                        myAnim.SetBool("Run", true);   
                    } else
                    {
                        playerState.SetState(UNITSTATE.WALK);
                        myAnim.SetBool("Walk", true);  
                        myAnim.SetBool("Run", false);   

                    }
                    
                    if(axisX > 0)
                    {
                        //spriteRenderer.flipX = false;
                        transform.localScale = new Vector3 (1 , 1, 1);
                    }else if(axisX < 0)
                    {
                        //spriteRenderer.flipX = true;
                        transform.localScale = new Vector3 (-1 , 1, 1);

                    }

                }else
                {
                    playerState.SetState(UNITSTATE.IDLE);
                    myAnim.SetBool("Walk", false);
                    myAnim.SetBool("Run", false);   
                }
        } 
    }

    public void HasLand()
    {
        theRB.gravityScale = 0;
        playerState.SetState(UNITSTATE.LAND);            
        isGround = true;
        transform.position = new Vector3 (transform.position.x , prevPosY, transform.position.z);
        // boxColliderStand.GetComponent<BoxCollider2D>().isTrigger = false;
        myAnim.SetBool("Jump", false);  
    }

    public void GetDirection()
    {
        float currentLocalScaleX = transform.localScale.x;
        if(currentLocalScaleX == 1)
        {
            playerDir =  DIRECTION.Right;
        }else
        {
            playerDir =  DIRECTION.Left;
        }
    }

    public void Run()
    {
        playerState.SetState(UNITSTATE.RUN);
    }

    public void onRun()
    {
         if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(ButtonCooler > 0 && ButtonCount == 1 && playerDir == DIRECTION.Right)
            {
                Run();
            }else if(playerDir == DIRECTION.Right)
            {
                ButtonCooler = 0.5f;
                ButtonCount += 1;
            }
            else
            {
                ButtonCooler = 0.5f;
                ButtonCount = 1; 
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(ButtonCooler > 0 && ButtonCount == 1 && playerDir == DIRECTION.Left)
            {
                Run();
            }else if(playerDir == DIRECTION.Left)
            {
                ButtonCooler = 0.5f;
                ButtonCount += 1;
            }
            else
            {
                ButtonCooler = 0.5f;
                ButtonCount = 1; 
            }
        }

        if(ButtonCooler > 0)
        {
            ButtonCooler -= Time.deltaTime;
        }else
        {
            ButtonCount = 0;
        }
    }

    public enum DIRECTION {
	Right,
	Left,
    }
}
