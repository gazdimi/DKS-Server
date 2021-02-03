using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// creates a character with hp and attribute for if they are in combat.
/// </summary>
public interface Character
{


    /// <summary>
    /// Sets combatant value to true.
    /// </summary>
    void enterCombat();

    /// <summary>
    /// Sets combatant value to false.
    /// </summary>
    void exitCombat();

    /// <summary>
    /// Returns the value of combatant.
    /// </summary>
    /// <returns></returns>
    bool InCombat();

    /// <summary>
    /// Decreases the character's hp by value
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    void TakeDamage(float damage);

    /// <summary>
    /// Destroys the character, using Destroy(gameObject,delay)
    /// </summary>
    void Kill(float delay);

    GameObject Closest(List<GameObject> targets);
}
