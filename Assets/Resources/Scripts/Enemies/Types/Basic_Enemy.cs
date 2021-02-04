using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Basic_Enemy:MonoBehaviour
{

    public float MOVEMENT_SPEED;
    public float ATTACK_SPEED;
    public float HEALTH;
    public float DAMAGE;
    public float RANGE;

    public float current_speed;
    public float current_attack_speed;
    public float current_health;
    public float current_range;
    public float current_damage;
    public float max_movement_speed;
    public List<Basic_Ability> abilities;
    public Modification_Base_Collection Modification_Bases;

    //
    public Basic_Ability readyAbility;
    public Basic_Ability activeAbility;
    public GameObject target;
    public Battle battle;
    //STATES
    public bool attacking;
    public bool preparingAttack;
    public bool stunned;




    public abstract void InitializeModificationBases();

    private void Awake()
    {
        InitializeModificationBases();
        if (gameObject.GetComponents<Basic_Ability>().Length > 0)
        {
            abilities = new List<Basic_Ability>(gameObject.GetComponents<Basic_Ability>());
        }
    }

    private void Update()
    {
        
        if (!stunned)
        {
            FindTarget();
            if (!preparingAttack&&!attacking)
            {
                MovementBehaviour();
            }
                    if (readyAbility != null)
             {
                if (!preparingAttack)
                {
                    if (!attacking)
                    {
                        if (readyAbility.PrerequisitesMet())
                        {
                            readyAbility.InitiateAttack();
                            activeAbility = readyAbility;
                            readyAbility = null;
                        }
                        else
                        {
                            if (target != null)
                            {
                                readyAbility.Behaviour();
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Applies all the modification Boosts.
    /// </summary>
    public void Apply_Modification_Boosts()
    {
        float totalmovementspeedboost = 0;
        float totalattackdamageboost = 0;
        float totalattackspeedboost = 0;
        float totalattackrangeboost = 0;
        foreach (GameObject mod in Modification_Bases.GetAllModifications())
        {
            totalmovementspeedboost += mod.GetComponent<Modification>().movement_speed;
            totalattackdamageboost += mod.GetComponent<Modification>().attack_damage;
            totalattackspeedboost += mod.GetComponent<Modification>().attack_speed;
            totalattackrangeboost += mod.GetComponent<Modification>().attack_range;
        }
        if (totalmovementspeedboost > 0)
        {
            max_movement_speed = MOVEMENT_SPEED * totalmovementspeedboost;
        }
        else
        {
            max_movement_speed = MOVEMENT_SPEED;
        }
        if (totalattackdamageboost > 0)
        {
            current_damage = DAMAGE * totalattackdamageboost;
        }
        else
        {
            current_damage = DAMAGE;
        }
        if (totalattackspeedboost > 0)
        {
            current_attack_speed = ATTACK_SPEED * totalattackspeedboost;
        }
        else
        {
            current_attack_speed = ATTACK_SPEED;
        }
        if (totalattackrangeboost > 0)
        {
            current_range = RANGE * totalattackrangeboost;
        }
        else
        {
            current_range = RANGE;
        }
    }
    /// <summary>
    /// Adds a modification.
    /// </summary>
    /// <param name="mod_type"></param>
    public void Add_Modification(string mod_type)
    {
        Modification mod = Modification_Prefab_Manager.GetInstance().SearchModification(mod_type).GetComponent<Modification>();
        foreach (Modification_Base mBase in Modification_Bases.GetAllBases())
        {
            if (mBase.AttachModification(mod))
            {
                Apply_Modification_Boosts();
                foreach (GameObject player in Battle_Manager.GetInstance().GetBattle(gameObject).GetPlayers())
                {
                    /*
                    if (player.GetComponent<LockOnTarget>().targetedCreature.Equals(gameObject))
                    {
                        player.GetComponent<LockOnTarget>().RefreshTargets();
                    }
                    */
                }
            }
        }
      
        
        return;
    }
    /// <summary>
    /// Removes the selected modification and the modification boosts.
    /// </summary>
    /// <param name="mod_to_remove"></param>
    public void Remove_Modification(GameObject mod_to_remove)
    {
        foreach(Modification_Base mbase in Modification_Bases.GetAllBases())
        {
            if (mbase.mod != null)
            {
                if (mbase.mod.Equals(mod_to_remove))
                {
                    mbase.DetachModification();
                    Apply_Modification_Boosts();
                    foreach (GameObject player in Battle_Manager.GetInstance().GetBattle(gameObject).GetPlayers())
                    {
                        if (player.GetComponent<LockOnTarget>().targetedCreature.Equals(gameObject))
                        {
                            player.GetComponent<LockOnTarget>().RefreshTargets();
                        }
                    }
                    return;
                }
            }
        }
    }
   
    /// <summary>
    /// Receives damage when damaged by player.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public bool TakeDamage(float damage)
    {
        Debug.Log("taking damage " + damage);
        current_health -= damage;
        if (current_health <= 0)
        {
            Battle_Manager.GetInstance().RemoveEnemy(this.gameObject);//Remove enemy from battle.
            Destroy(gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForReadyAttack()
    {
        if (abilities.Count > 0)
        {
            foreach(Basic_Ability ability in abilities)
            {
                if (ability.cdReady)
                {
                    readyAbility = ability;
                    return;
                }
            }
        }
        readyAbility = null;
    }

    protected void Stun()
    {
        stunned = true;
        Invoke("Unstun", 3f);
    }
    protected void Unstun()
    {
        stunned = false;
    }

    protected void FindTarget()
    {
        target = TargetManager.GetInstance().GetClosestTarget(gameObject, battle.GetPlayers());
        return;
    }

    protected abstract void MovementBehaviour();
}
