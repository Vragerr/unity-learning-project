using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    Controller controller;
    public int health;
    public int maxHealth;
    public float gold;
    public float Strength;
    public float Power;
    public float AttackSpeed;
    public float MovementSpeed;
    public float jumpForce;
    public UIController UIController;
    private void Start()
    {
        controller = GetComponent<Controller>();
        maxHealth = 120;
        health = maxHealth;
        UIController.SetMaxHealth(health);
        MovementSpeed = 5f;
        AttackSpeed = 1;
        jumpForce = 12f;
    }


    void Update()
    {
        UIController.SetHealth(health);
        if (Input.GetKeyDown(KeyCode.N)) TakeDamage();
        if (Input.GetKeyDown(KeyCode.B)) HealHP();
    }
    public void TakeDamage()
    {
        if (health > 20)
            health -= 20;
        else health = 0;
    }
    void HealHP()
    {
        health += 20;
        controller.isDied = false;
    }
}
