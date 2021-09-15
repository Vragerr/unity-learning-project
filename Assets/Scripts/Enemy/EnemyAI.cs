using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy { 
    public class EnemyAI : MonoBehaviour
    {
        private float attackSpeed, attackColdown,knockbackStart;
        [SerializeField]
        private int MaxHitpoints;
        [SerializeField]
        private bool applyKnockback;
        [SerializeField]
        private float knockbackSpeedX, knockbackSpeedY,knockbackDuration;

        private int currentHealth,playerFacingDirection;


        public Collider2D
            bodyCollider,        //body collider
            AttackCollider,      // Attack checkPlayer collider
            SightCollider,       //sight Collider
            colliderEnemyGround, //GroundCollider
            colliderCharacter; //player collider
       public Transform 
            target,         // target transform
            groundCheckpos; //Ground check Position for detect ended platform

        public Rigidbody2D rb; 
        public LayerMask groundLayer;
        public Animator animator;
        private Controller playerController;

        public float 
            moveSpeed,
            timerAttack;
        private bool mustPatrol; //Patrol Mod on/off
        private bool mustTurn,knockback,isDead,playerOnLeft,isAttack;

        void Start()
        {
            attackSpeed = 5f;
            moveSpeed = 120f;
            attackColdown = 1f;
            MaxHitpoints = 100;
            currentHealth = MaxHitpoints;
            timerAttack = 1f;
            mustPatrol = true;
           // target = PlayerManager.instance.player.transform;
            AttackCollider = GetComponent<Collider2D>();
            animator = GetComponent<Animator>();
            SightCollider = GetComponent<Collider2D>();           
            Physics2D.IgnoreCollision(colliderEnemyGround, colliderCharacter);
            Physics2D.IgnoreCollision(bodyCollider, colliderCharacter);
            playerController = GameObject.Find("Player").GetComponent<Controller>();
        }

        void Update()
        {
            if (!isDead)
            {
                CheckMustPatrol();
                CheckKnockback();
                if (timerAttack < 3)
                    timerAttack += Time.deltaTime * attackColdown;
                OnTriggerEnter2D(AttackCollider);
                //mustPatrol = true;
                if (mustPatrol)
                {
                    Patrol();
                }
            }
                   
        }

        private void FixedUpdate()
        {
            if (mustPatrol)
            {
                mustTurn = !Physics2D.OverlapCircle(groundCheckpos.position, 0.1f, groundLayer);
            }
        }

        void Patrol()
        {
            if (mustTurn || bodyCollider.IsTouchingLayers(groundLayer))
            {
                Flip();
            }
            animator.SetInteger("AnimState", 2);
            rb.velocity = new Vector2(moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
            float distance = Vector2.Distance(target.position, transform.position);
            Vector2 directionToTarget = transform.position - target.position;
            float angle = Vector2.Angle(transform.forward, directionToTarget);
            float distance_direct = directionToTarget.magnitude;
            if (Mathf.Abs(angle) < 90 && distance_direct < 10)
                Debug.Log("Target is front "+ distance_direct);
        }

        void Flip()
        {
            mustPatrol = false;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            moveSpeed *= -1;
            mustPatrol = true;
        }

        private void OnTriggerEnter2D(Collider2D attackCollider)
        {
            if (attackCollider.transform.tag == "Player" && !isDead)
            {
                mustPatrol = false;
                if (timerAttack >1f) 
                { 
                animator.SetInteger("AnimState", 1);
                animator.SetTrigger("Attack");
                Debug.LogWarning("ATTACK");
                attackCollider.GetComponent<PlayerStats>().TakeDamage();
                timerAttack = 0f;
                }
                mustPatrol = true;
            }
        } 
        private void Damage(int damageValue)
        {
            currentHealth -= damageValue;
            playerFacingDirection = playerController.GetFacingDirrection();
            animator.SetTrigger("Hurt");
            if (playerFacingDirection > 0)
            {
                playerOnLeft = true;
            }
            else
            {
                playerOnLeft = false;
            }
            if(applyKnockback && currentHealth > 0)
            {
                Knockback();
            }
            if(currentHealth <= 0)
            {
                Die();
            }
        }
        private void Knockback()
        {
            knockback = true;            
            mustPatrol = false;
            knockbackStart = Time.time;
            rb.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
        }
        private void CheckKnockback()
        {
            if(knockback && Time.time >= knockbackStart + knockbackDuration)
            {
                mustPatrol = true;
                knockback = false;
                rb.velocity = new Vector2(0.0f, rb.velocity.y);
            }
        }
        private void CheckMustPatrol()
        {
            if (!knockback && !isAttack)
                mustPatrol = true;
        }
        private void Die()
        {
            isDead = true;
            mustPatrol = false;
            animator.SetTrigger("Death");
            DisableBearColliders();
        }
        public void DisableBearColliders()
        {
            foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
            {
                c.enabled = false;  
            }
        }

    }
}