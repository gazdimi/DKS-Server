using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy:Basic_Enemy
{
    public override void InitializeModificationBases()
    {
        Modification_Bases = new Modification_Base_Collection();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Base"))
            {
                Modification_Bases.AddModificationBase(child.GetComponent<Modification_Base>());
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (activeAbility != null)
        {
            if (collision.transform.gameObject.tag.Equals("Player"))
            {
                collision.transform.gameObject.GetComponent<Player>().TakeDamage(activeAbility.DealDamage());
            }
            else if (collision.transform.gameObject.tag.Equals("Wall"))
            {
                if (activeAbility.abilityname.Equals("Bash_Attack"))
                {
                    Stun();
                    activeAbility.EndAttack();
                }
            }
        }
    }

    protected override void MovementBehaviour()
    {
        
    }

}
