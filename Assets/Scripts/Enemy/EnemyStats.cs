using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    class EnemyStats : MonoBehaviour
    {
        public EnemyStats(){
            this.attackSpeed = 5f;
            this.moveSpeed = 3f;
            this.attackColdown = 1f;
            this.hitPoints = 100;
        }
        public float attackSpeed, moveSpeed,attackColdown;
        private int hitPoints;
        public float GetAttackSpeed()
        {
            return this.attackSpeed;
        }
        public void TakeDamage()
        {
            if (this.hitPoints > 20)
                this.hitPoints -= 20;         
        }
        

    }

}
