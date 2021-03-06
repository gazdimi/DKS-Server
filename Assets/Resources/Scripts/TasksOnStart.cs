﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TasksOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("DungeonScene"))
        {
            Send.GenerateDungeon();
            PlayerManager.GetInstance().SpawnPlayers();

            List<GameObject> activePlayers = PlayerManager.GetInstance().GetActivePlayers();
            for (int i = 0; i < activePlayers.Count; i++)
            {
                Send.Teleport(activePlayers[i].GetComponent<ServerPlayer>());
            }

            SpawnWeapon.Spawn("Handgun", new Vector3(30, 1.5f, -30));
            SpawnWeapon.Spawn("Handgun", new Vector3(30, 1.5f, -20));
            SpawnWeapon.Spawn("Sword", new Vector3(20, 1.5f, -30));
        }
        
    }
}
