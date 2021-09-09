using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{   
    enum State
    {
        Walking,
        Died,
        Knockback,
        Jumping,
        Idle
    }
    private State state;
    Rigidbody2D rb;
    PlayerStats stats;
    private Animator animator;
    private float
        timerAttack,    //time to attack
        movementDirection,          // direct move
        DelayToRunAnim; 
    public Transform GroundCheck;
    private float checkRadius = 0.2f;
    public LayerMask Ground;
    public int
        amountJumpsLeft,
        amountJump;
    public bool
        isCanAttack = true,
        isGrounded,   // is on Ground
        isDied,       // is Player are dead
        canMove;
    void checkingGround()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, Ground);
        animator.SetBool("Grounded", isGrounded);
    }
    void Start()
    {
        
        amountJumpsLeft=amountJump = 1;
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        checkingGround();
        CheckInputs();
        CheckCanJump();
    }

    void FixedUpdate()
    {
        #region Movement
        if (timerAttack<3f) 
            timerAttack += Time.deltaTime*stats.AttackSpeed;
        //Movement
        ApplyMovement();
        //Direction
        if (movementDirection > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if(movementDirection<0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        
        //Run animation
        if ((Mathf.Abs(movementDirection) > Mathf.Epsilon)&& isGrounded)
        {
            state = State.Walking;
            DelayToRunAnim = 0.05f;
            animator.SetInteger("AnimState", 1);
        }
        else if(isGrounded)
        {
            state = State.Idle;
            DelayToRunAnim -= Time.deltaTime;
            if (DelayToRunAnim < 0)
                animator.SetInteger("AnimState", 0);
        }

        //Set AirSpeed
        animator.SetFloat("AirSpeedY", rb.velocity.y);
        #endregion
        #region others
        //Attack
        if (Input.GetKey(KeyCode.E))
        {
            Attack();
        }
        //Die
        if (stats.health <= 0)
        {
            Die();
        }      
        #endregion
    }
    #region CheckInputs
    private void CheckInputs()
    {
        movementDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || amountJumpsLeft>0)
            {
                state = State.Jumping;
                Jump(rb, stats.jumpForce);
            }
            else
            {
                
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y*0.5f);
        }

    }
    #endregion
    #region MovementMethod
    private void ApplyMovement()
    {
        Vector2 movement;
        if (!isGrounded && movementDirection==0)
        {
            movement = new Vector2(rb.velocity.x, rb.velocity.y);
        }
        else
        {
            movement = new Vector2(movementDirection * stats.MovementSpeed, rb.velocity.y);
        }       
        rb.velocity = movement;
    }

    #endregion
    #region Die
    void Die()
    {
        if (!isDied)
        {
            Debug.Log("Died");
            animator.SetBool("noBlood", false);
            animator.SetTrigger("Death");
            isDied = true;
        }
    }
    #endregion
    #region JumpMethod
    void Jump(Rigidbody2D rb,float jumpForce)
    {
        //rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetTrigger("Jump");
        amountJumpsLeft--;
        animator.SetBool("Grounded", isGrounded);
        isGrounded = false;
    }
    #endregion
    #region CheckCanJump
    private void CheckCanJump()
    {
        if(isGrounded && rb.velocity.y <= 0.01f)
        {
            amountJumpsLeft = amountJump;
        }
    }
    #endregion
    #region Attack
    public void Attack()
    {
        if (timerAttack > 1f) isCanAttack = true; else isCanAttack = false;
        if (state ==State.Idle && isCanAttack)
        {
            animator.SetTrigger("Attack"+1);
            timerAttack = 0f;
        }
        else if (isCanAttack && state==State.Jumping)
        {
            animator.SetTrigger("Attack" + 3);
            timerAttack = 0f;
        }
    }
    #endregion
}
