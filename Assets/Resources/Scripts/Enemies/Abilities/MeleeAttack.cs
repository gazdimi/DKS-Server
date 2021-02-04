using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Basic_Ability
{
    bool inrange = false;
    private void Awake()
    {
        abilityname = "Melee_Attack";
        timeToEnd = 0.2f;
        cooldown = 2f;
        cdReduction = 0.5f;
        damageMultiplier = 1f;
        timeRemaining = cooldown;
        executingAttack = false;
        cdReady = false;
        enemyScript = this.gameObject.GetComponent<Basic_Enemy>();
    }
    public override void EndAttack()
    {
        executingAttack = false;
        enemyScript.attacking = false;
        enemyScript.activeAbility = null;
    }
    private void Update()
    {
        CoolDownTime();
        if (enemyScript.target != null)
        {
            if (Vector3.Distance(gameObject.transform.position, enemyScript.target.transform.position) <3) inrange = true;
            else inrange = false;
        }
    }

    public override void InitiateAttack()
    {
        cdReady = false;
        executingAttack = true;
        enemyScript.preparingAttack = true;
        BeginAttack();
    }
    private void BeginAttack()
    {
        Debug.Log("Executing Melee Attack");
        enemyScript.preparingAttack = false;
        enemyScript.attacking = true;
       // enemyScript.target.GetComponent<Player>().TakeDamage(DealDamage());
        Invoke("EndAttack", timeToEnd);
    }

    public override bool PrerequisitesMet()
    {
        return cdReady && inrange;
    }
    public override void Behaviour()
    {
        if (enemyScript.current_speed < enemyScript.max_movement_speed)
        {
            enemyScript.current_speed += enemyScript.max_movement_speed * 0.01f;
        }
        transform.LookAt(new Vector3(enemyScript.target.transform.position.x,gameObject.transform.position.y, enemyScript.target.transform.position.z));
        transform.position += transform.forward * enemyScript.current_speed * Time.deltaTime;
    }
}
