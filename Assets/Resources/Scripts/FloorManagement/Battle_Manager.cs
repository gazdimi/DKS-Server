using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Manager
{
    #region Singleton
    private static Battle_Manager instance = null;
    public static Battle_Manager GetInstance()
    {
        if (instance == null)
        {
            return new Battle_Manager();
        }
        return instance;
    }
    private Battle_Manager()
    {
        instance = this;
        activeBattles = new List<Battle>();
    }
    #endregion

    List<Battle> activeBattles;
    /// <summary>
    /// Adds a new battle to the battle collection.
    /// </summary>
    /// <param name="battle"></param>
    public void AddBattle(Battle battle)
    {
        activeBattles.Add(battle);
    }
    /// <summary>
    /// Removes an existing battle from the battle collection.
    /// </summary>
    /// <param name="battle"></param>
    public void RemoveBattle(Battle battle)
    {
        foreach(GameObject player in battle.GetPlayers())
        {
            player.GetComponent<Player>().exitCombat();
        }
        battle.GetRoom().OpenDoors();
        activeBattles.Remove(battle);
    }
    /// <summary>
    /// Returns all the active battles.
    /// </summary>
    /// <returns></returns>
    public List<Battle> getActiveBattles()
    {
        return activeBattles;
    }
    /// <summary>
    /// Removes enemy from a battle.
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(GameObject enemy)
    {
        foreach(Battle battle in activeBattles)
        {
            if (battle.GetEnemies().Contains(enemy))
            {
                battle.RemoveEnemy(enemy);
                return;
            }
        }
    }
    /// <summary>
    /// Checks if there is a battle going on in the selected room.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public bool BattleExists(Basic_Room room)
    {
        foreach(Battle battle in activeBattles)
        {
            if (battle.GetRoom().Equals(room))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Returns the on-going battle in the selected room.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public Battle GetBattle(Basic_Room room)
    {
        foreach (Battle battle in activeBattles)
        {
            if (battle.GetRoom().Equals(room))
            {
                return battle;
            }
        }
        return null;
    }
    /// <summary>
    /// Returns the on-going battle in the selected room.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public Battle GetBattle(GameObject entity)
    {
        foreach (Battle battle in activeBattles)
        {
            if (battle.GetPlayers().Contains(entity)||battle.GetEnemies().Contains(entity))
            {
                return battle;
            }
        }
        return null;
    }
}
