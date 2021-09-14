using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy { 
    public class EnemyAI : MonoBehaviour
    {
        //For future animationsCoreController
        private enum State
        {
            Walking,
            Knockback,
            Dead
        }
        private State currentState;
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
        //stats controll
        EnemyStats EnemyStat = new EnemyStats();
        public float 
            walkSpeed,
            timerAttack; 
        private bool 
            mustPatrol, //Patrol Mod on/off
            mustTurn;   
        void Start()
        {
            timerAttack = 1f;
            mustPatrol = true;
            walkSpeed *= EnemyStat.moveSpeed;
           // target = PlayerManager.instance.player.transform;
            AttackCollider = GetComponent<Collider2D>();
            animator = GetComponent<Animator>();
            SightCollider = GetComponent<Collider2D>();           
            Physics2D.IgnoreCollision(colliderEnemyGround, colliderCharacter);
            Physics2D.IgnoreCollision(bodyCollider, colliderCharacter);
        }

        void Update()
        {
            if (timerAttack < 3)
                timerAttack += Time.deltaTime*EnemyStat.attackColdown;                     
                OnTriggerEnter2D(AttackCollider);                    
                //mustPatrol = true;
                if (mustPatrol)
                {
                    Patrol();
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
            rb.velocity = new Vector2(walkSpeed * Time.fixedDeltaTime, rb.velocity.y);
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
            walkSpeed *= -1;
            mustPatrol = true;
        }

        private void OnTriggerEnter2D(Collider2D attackCollider)
        {
            if (attackCollider.transform.tag == "Player" )
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
    }
}