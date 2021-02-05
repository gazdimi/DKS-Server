using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Basic_Ability
{
    public GameObject bullet;
    private void Awake()
    {
        abilityname = "Ranged_Attack";
        timeToEnd = 0.2f;
        cooldown = 2f;
        cdReduction = 0.5f;
        damageMultiplier = 1f;
        timeRemaining = cooldown;
        executingAttack = false;
        cdReady = false;
        enemyScript = this.gameObject.GetComponent<Basic_Enemy>();
    }
    private void Update()
    {
        if (enemyScript.target != null)
        {
            transform.rotation = Quaternion.LookRotation(enemyScript.target.transform.position - transform.position);
            Send.MoveEnemy(transform.position, transform.rotation, ConnectionEnemyHandler.GetInstance().allExistingEnemies.FindIndex(x => x == gameObject));
        }

        CoolDownTime();
    }
    public override void Behaviour()
    {
        if (Vector3.Distance(gameObject.transform.position, enemyScript.target.transform.position) > enemyScript.current_range)
        {
            if (enemyScript.current_speed < enemyScript.max_movement_speed)
            {
                enemyScript.current_speed += enemyScript.max_movement_speed * 0.01f;
            }
            
            transform.position += transform.forward * enemyScript.current_speed * Time.deltaTime;
            Send.MoveEnemy(transform.position, transform.rotation, ConnectionEnemyHandler.GetInstance().allExistingEnemies.FindIndex(x => x == gameObject));
        }
    }

    public override void EndAttack()
    {
        executingAttack = false;
        enemyScript.attacking = false;
        enemyScript.activeAbility = null;
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
        Debug.Log("Executing Ranged Attack");
        enemyScript.preparingAttack = false;
        enemyScript.attacking = true;
        GameObject bullet = Instantiate(this.bullet, gameObject.transform.position+transform.forward*2, Quaternion.identity);
        bullet.GetComponent<Enemy_Bullet>().damage = enemyScript.DAMAGE * damageMultiplier;
        bullet.transform.rotation = transform.rotation;
        Invoke("EndAttack", timeToEnd);
    }


    public override bool PrerequisitesMet()
    {
        return cdReady&& Vector3.Distance(gameObject.transform.position, enemyScript.target.transform.position) <= enemyScript.current_range;
    }

}
