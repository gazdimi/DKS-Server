using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Prefab_Manager
{
    #region Singleton
    private static Enemy_Prefab_Manager instance = null;
    public static Enemy_Prefab_Manager GetInstance()
    {
        if (instance == null)
        {
            return new Enemy_Prefab_Manager();
        }
        return instance;
    }
    private Enemy_Prefab_Manager()
    {
        instance = this;
        meleeEnemies = LoadMelleeEnemies();
        rangedEnemies = LoadRangedEnemies();
    }
    #endregion

    List<GameObject> meleeEnemies = new List<GameObject>();
    List<GameObject> rangedEnemies = new List<GameObject>();

    /// <summary>
    /// Loads all the melee enemy prefabs.
    /// </summary>
    /// <returns></returns>
    private List<GameObject> LoadMelleeEnemies()
    {
       return new List<GameObject>((Resources.LoadAll<GameObject>("Prefabs/Enemies/Melee")));
    }

    /// <summary>
    /// Loads all the ranged enemy prefabs.
    /// </summary>
    /// <returns></returns>
    private List<GameObject> LoadRangedEnemies()
    {
        return new List<GameObject>((Resources.LoadAll<GameObject>("Prefabs/Enemies/Ranged")));
    }
    
    /// <summary>
    /// Returns all current melee prefabs.
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetMeleeEnemies()
    {
        return meleeEnemies;
    }

    /// <summary>
    /// Returns all current ranged prefabs.
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetRangedEnemies()
    {
        return rangedEnemies;
    }



}

