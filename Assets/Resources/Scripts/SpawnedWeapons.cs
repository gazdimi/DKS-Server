using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedWeapons : MonoBehaviour
{
    public static Dictionary<int, SpawnedWeapons> spawned_weapons = new Dictionary<int, SpawnedWeapons>();
    private static int next_weapon_id = 1;

    public int weapon_id;
    //public bool holded = false;

    private void Start()
    {
        //holded = false;
        weapon_id = next_weapon_id;
        next_weapon_id++;
        spawned_weapons.Add(weapon_id, this);
    }
}
