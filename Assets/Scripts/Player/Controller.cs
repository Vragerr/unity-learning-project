using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{   
    public enum State
    {
        Walking,
        Died,
        Knockback,
        Jumping,
        Idle,
        Falling,
        Attack
    }
    public State state = new State();
    Rigidbody2D rb;
    PlayerStats stats;

    private Animator animator;

    private float timerAttack;   //time to attack
    private float movementDirection;         // direct move
    private float DelayToRunAnim;
    private float checkRadius = 0.2f;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;

    public float dashTime =0.2f;
    public float dashSpeed = 50f;
    public float distanceBetweenImages = 0.1f;
    public float dashCooldown = 2.5f;
    public Transform GroundCheck;

    public bool isDied;      // is Player are dead

    public LayerMask Ground;
    public int amountJumpsLeft;
    public int amountJump;

    private bool isCanAttack = true;
    private bool isGrounded;   // is on Ground
    private bool canMove;
    private bool isDashing;

    void checkingGround()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, Ground);
        animator.SetBool("Grounded", isGrounded);  
    }
    void Start()
    {
        canMove = true;
        amountJumpsLeft=amountJump = 1;
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!isDied) { 
        checkingGround();
        CheckInputs();
        CheckCanJump();
        CheckDash();
        }
    }

    void FixedUpdate()
    {
        if (!isDied)
        {
            if (timerAttack < 3f)
                timerAttack += Time.deltaTime * stats.AttackSpeed;
            ApplyMovement();
            AnimationsControl();
            //Die
            if (stats.health <= 0)
            {
                Die();
            }
        }
    }

    private void AnimationsControl()
    {
        animator.SetFloat("AirSpeedY", rb.velocity.y);             //Set AirSpeed
        if ((Mathf.Abs(movementDirection) > Mathf.Epsilon) && isGrounded)
        {
            state = State.Walking;
            state.Equals(State.Walking);
            DelayToRunAnim = 0.05f;
            animator.SetInteger("AnimState", 1);
        }
        else if (isGrounded)
        {
            state = State.Idle;
            state.Equals(State.Idle);
            DelayToRunAnim -= Time.deltaTime;
            if (DelayToRunAnim < 0)
                animator.SetInteger("AnimState", 0);
        }

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
        if (Input.GetButtonDown("Dash"))
        {
            if(Time.time >=(lastDash+dashCooldown))
            AttemptToDash();
        }
        if (Input.GetButtonDown("Attack"))
        {
            Attack();
        }

    }
    #endregion
    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    #region MovementMethod
    private void ApplyMovement()
    {
        if (canMove)
        {
            Vector2 movement;
            if (!isGrounded && movementDirection == 0)
            {
                movement = new Vector2(rb.velocity.x, rb.velocity.y);
            }
            else
            {
                movement = new Vector2(movementDirection * stats.MovementSpeed, rb.velocity.y);
            }
            rb.velocity = movement;
        }


        if (movementDirection > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (movementDirection < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    #endregion
    #region Die
    void Die()
    {
        if (!isDied)
        {
            Debug.Log("Died");
            canMove = false;
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
    #region Checkers
    private void CheckCanJump()
    {
        if(isGrounded && rb.velocity.y <= 0.01f)
        {
            amountJumpsLeft = amountJump;
        }
    }
    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                rb.velocity = new Vector2(dashSpeed * movementDirection,0);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
                canMove = true;
            }
        }
    }
    #endregion
    #region Attack
    public void Attack()
    {
        isCanAttack = timerAttack > 1f ? true : false;
        if (isCanAttack)
        {
            if (state == State.Idle)
            {
                animator.SetTrigger("Attack" + 1);
            }
            else if (state == State.Walking)
            {
                animator.SetTrigger("Attack" + 2);
            }
            else if (!isGrounded)
            {
                animator.SetTrigger("Attack" + 3);
            }
            timerAttack = 0f;
        }
    }
    #endregion
}
