using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BashingAttack : Basic_Ability
{
    private Vector3 targetDirection;

    private void Awake()
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                         
        abilityname = "Bash_Attack";
        timeToAttack = 2f;
        timeToEnd = 3f;
        cooldown = 10f;
        cdReduction = 0.9f;
        damageMultiplier = 1.5f;
        timeRemaining = cooldown;
        executingAttack = false;
        cdReady = false;
        enemyScript = gameObject.GetComponent<Basic_Enemy>();
    }
    private void Update()
    {
        CoolDownTime();
        if (executingAttack)
        {
            transform.position += transform.forward *enemyScript.max_movement_speed*2* Time.deltaTime;
        }
    }

    public override void InitiateAttack()
    {
        Debug.Log("Executing Bash Attack");
        cdReady = false;
        enemyScript.preparingAttack = true;
        enemyScript.current_speed = 0;
        transform.LookAt(new Vector3(enemyScript.target.transform.position.x, gameObject.transform.position.y, enemyScript.target.transform.position.z));
        Invoke("BeginAttack",timeToAttack);
    }
    public override void EndAttack()
    {
        executingAttack = false;
        enemyScript.attacking = false;
        enemyScript.activeAbility = null;
    }
    private void BeginAttack()
    {
        executingAttack = true;
        enemyScript.preparingAttack = false;
        enemyScript.attacking = true;
    }

    public override bool PrerequisitesMet()
    {
        return cdReady;
    }

    public override void Behaviour()
    {
    }
}
