using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWeapon
{
    public static void SpawnMelee(string weapon, Vector3 location)
    {
        List<GameObject> meleeweapons = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Equipment/Weapons/Melee"));
        Spawn(weapon, meleeweapons, location);
    }

    public static void SpawnRanged(string weapon, Vector3 location)
    {
        List<GameObject> rangedweapons = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Equipment/Weapons/Ranged"));
        Spawn(weapon, rangedweapons, location);
    }

    static void Spawn(string weapon, List<GameObject> weapons, Vector3 location)
    {
        foreach(GameObject prefab in weapons)
        {
            if (prefab.name.Equals(weapon))
            {
                GameObject.Instantiate(prefab, location, new Quaternion());
                break;
            }
        }
    }
}
