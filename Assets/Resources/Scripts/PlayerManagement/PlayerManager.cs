using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    #region Singleton
    private static PlayerManager instance = null;
    public static PlayerManager GetInstance()
    {
        if (instance == null)
        {
            return new PlayerManager();
        }
        return instance;
    }
    private PlayerManager()
    {
        instance = this;
        activePlayers = new List<GameObject>();
    }
    #endregion
    List<GameObject> activePlayers;
    public List<GameObject> GetActivePlayers()
    {
        return activePlayers;
    }
    public void AddPlayer(GameObject player)
    {
        if (!activePlayers.Contains(player))activePlayers.Add(player);
    }
    public void RemovePlayer(GameObject player)
    {
        if (activePlayers.Contains(player)) activePlayers.Remove(player);
    }
    public void SpawnPlayers()
    {
        int pCount = 0;
        foreach(GameObject player in activePlayers)
        {
            switch(pCount)
            {
                case 0:
                    player.transform.position = new Vector3(22f, 1, -28f);
                    break;
                case 1:
                    player.transform.position = new Vector3(28f, 1, -28f);
                    break;
                case 2:
                    player.transform.position = new Vector3(22f, 1, -22f);
                    break;
                case 3:
                    player.transform.position = new Vector3(28f, 1, -22f);
                    break;
            }
            pCount++;
        }
    }
    public void DespawnPlayers()
    {
        foreach (GameObject player in activePlayers)
        {
            player.transform.position = player.transform.position - new Vector3(0, -3, 0);
           // Object.Destroy(player);
        }
    }


}
