using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public abstract class Basic_Ability:MonoBehaviour
{
    public string abilityname;
    public bool cdReady;
    public bool ready;
    protected float cooldown;
    protected float cdReduction;
    protected float damageMultiplier;//Need to make damage multiplier to be applied
    protected float timeRemaining;
    protected float timeToAttack;
    protected float timeToEnd;
    protected Basic_Enemy enemyScript;
    protected bool executingAttack;
    /// <summary>
    /// Countdown timer for enemy cooldown.
    /// </summary>
    protected void CoolDownTime()
    {
        if (cdReady)
        {
            enemyScript.CheckForReadyAttack();
        }
        else
        {
            if (!executingAttack)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }
                else
                {
                    cdReady = true;
                    Debug.Log(cooldown - (cooldown * enemyScript.current_attack_speed * cdReduction / 10));
                    timeRemaining = cooldown-(cooldown*enemyScript.current_attack_speed*cdReduction/100);
                    enemyScript.CheckForReadyAttack();
                }
            }

        }
    }
    /// <summary>
    /// Enemy prepares the attack.
    /// </summary>
    public abstract void InitiateAttack();
    /// <summary>
    /// Behaviour needed for the attack to be executed.
    /// </summary>
    public abstract void Behaviour();
    /// <summary>
    /// End the attack.
    /// </summary>
    public abstract void EndAttack();
    /// <summary>
    /// Check if the attack requirements are met.
    /// </summary>
    /// <returns></returns>
    public abstract bool PrerequisitesMet();
    /// <summary>
    /// Returns the total amount of damage to deal to a player.
    /// </summary>
    /// <returns></returns>
    public float DealDamage()
    {
        return damageMultiplier * enemyScript.current_damage;
    }
}
