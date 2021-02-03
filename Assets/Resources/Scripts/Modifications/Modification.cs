using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modification : MonoBehaviour
{
    public float mod_health;
    public string type;
    public string size;
    public float movement_speed;
    public float attack_speed;
    public float attack_damage;
    public float attack_range;

    /// <summary>
    /// Take damage to health.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public bool TakeDamage(float damage)
    {
        mod_health -= damage;
        if (mod_health <= 0)
        {
            gameObject.GetComponentInParent<Basic_Enemy>().Remove_Modification(gameObject);
            gameObject.transform.SetParent(null);
            return true;
        }
        else
        {
            return false;
        }
    }

}
