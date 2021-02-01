using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modification_Prefab_Manager
{
    #region Singleton
    private static Modification_Prefab_Manager instance = null;
    public static Modification_Prefab_Manager GetInstance()
    {
        if (instance == null)
        {
            return new Modification_Prefab_Manager();
        }
        return instance;
    }
    private Modification_Prefab_Manager()
    {
        instance = this;
        LoadModificationPrefabs();
    }
    #endregion
    private List<GameObject> Movement_Speed_Modifications;
    private List<GameObject> Attack_Damage_Modifications;
    private List<GameObject> Attack_Range_Modifications;
    private List<GameObject> Attack_Speed_Modifications;
    /// <summary>
    /// Loads all the modification prefabs.
    /// </summary>
    /// <returns></returns>
    public  bool LoadModificationPrefabs()
    {
        try
        {
            Movement_Speed_Modifications = new List<GameObject>((Resources.LoadAll<GameObject>("Prefabs/Modifications/Movement_Speed_Modifications"))); // Load all speed modification prefabs from the project folder.
            Attack_Damage_Modifications = new List<GameObject>((Resources.LoadAll<GameObject>("Prefabs/Modifications/Attack_Damage_Modifications"))); // Load all attack damage modification prefabs from the project folder.
            Attack_Range_Modifications = new List<GameObject>((Resources.LoadAll<GameObject>("Prefabs/Modifications/Attack_Range_Modifications"))); // Load all attack range modification prefabs from the project folder.
            Attack_Speed_Modifications = new List<GameObject>((Resources.LoadAll<GameObject>("Prefabs/Modifications/Attack_Speed_Modifications"))); // Load all attack speed modification prefabs from the project folder.
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Returns the speed modification prefabs list.
    /// </summary>
    /// <returns></returns>
    public  List<GameObject> GetAllSpeedModifications()
    {
        return Movement_Speed_Modifications;
    }
    /// <summary>
    /// Search for a modification in the prefabs.
    /// </summary>
    /// <param name="mod_name"></param>
    /// <returns></returns>
    public  GameObject SearchModification(string mod_name)
    {
        if (mod_name.Contains("Movement_Speed"))
        {
            for(int i = 0; i < Movement_Speed_Modifications.Count; i++)
            {
                if (Movement_Speed_Modifications[i].name == mod_name)
                {
                    return Movement_Speed_Modifications[i];
                }
            }
        }
        else if (mod_name.Contains("Attack_Damage"))
        {
            for (int i = 0; i < Attack_Damage_Modifications.Count; i++)
            {
                if (Attack_Damage_Modifications[i].name == mod_name)
                {
                    return Attack_Damage_Modifications[i];
                }
            }
        }
        else if (mod_name.Contains("Attack_Range"))
        {
            for (int i = 0; i < Attack_Range_Modifications.Count; i++)
            {
                if (Attack_Range_Modifications[i].name == mod_name)
                {
                    return Attack_Range_Modifications[i];
                }
            }
        }
        else if (mod_name.Contains("Attack_Speed"))
        {
            for (int i = 0; i < Attack_Speed_Modifications.Count; i++)
            {
                if (Attack_Speed_Modifications[i].name == mod_name)
                {
                    return Attack_Speed_Modifications[i];
                }
            }
        }
        return null;
    }
}
