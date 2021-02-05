using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Basic_Enemy
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

    protected override void MovementBehaviour()
    {
        if (target != null)
        {
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) < current_range * 0.8)
            {
                transform.position = Vector3.MoveTowards(gameObject.transform.position, target.transform.position, -1 * current_speed * Time.deltaTime);
                Send.MoveEnemy(transform.position, transform.rotation, ConnectionEnemyHandler.GetInstance().allExistingEnemies.FindIndex(x => x == gameObject));
            }
        }
    }
}
