using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField]
    private bool combatEnabled;
    [SerializeField]
    private float inputTimer,attackRadius,attackDamage;
    [SerializeField]
    private Transform attackHitBoxPos;
    [SerializeField]
    private LayerMask damageableLayers;

    private bool gotInput,isAttacking, isCanAttack;

    private float lastInputTime=Mathf.NegativeInfinity;
    private float timerAttack;
    private string state;

    private Animator animator;
    private PlayerStats stats;
    private Controller controller;

    private void Start()
    {
        controller = GetComponent<Controller>();
        stats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        state = controller.state.ToString();
        if (!controller.isDied) {
            if (timerAttack < 3f)
                timerAttack += Time.deltaTime * stats.AttackSpeed;
            checkCombatInput();
            CheckAttacks();
        }
    }
    private void checkCombatInput()
    {
        if (Input.GetButtonDown("Attack"))
        {
            if (combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }
    private void CheckAttacks()
    {
        if (gotInput)
        {
            isCanAttack = timerAttack > 1f ? true : false;
            if (!isAttacking && isCanAttack)
            {              
                timerAttack = 0f;
                gotInput = false;
                isAttacking = true;             
                if (state == "Idle")
                {
                    animator.SetTrigger("Attack" + 1);
                }
                else if (state == "Walking")
                {
                    animator.SetTrigger("Attack" + 2);
                }
                else if (!controller.isGrounded)
                {
                    animator.SetTrigger("Attack" + 3);
                }
                FinishAttack();
            }
        }
        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }
    private void CheckAttackHitBox()
    {
       Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitBoxPos.position,attackRadius,damageableLayers);
        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage",attackDamage);
        }
    }
    private void FinishAttack()
    {
        CheckAttackHitBox();
        isAttacking = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitBoxPos.position, attackRadius);
    }
}
