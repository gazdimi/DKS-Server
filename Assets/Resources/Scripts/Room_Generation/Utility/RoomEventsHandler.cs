using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEventsHandler : MonoBehaviour
{
    private Basic_Room thisRoom;
    private void Start()
    {
        foreach (Basic_Room room in DungeonGenerator.GetInstance().allrooms)
        {
            if (room.RoomObject.Equals(gameObject))
            {
                thisRoom = room;
                break;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Battle_Manager.GetInstance().BattleExists(thisRoom))
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                Battle_Manager.GetInstance().GetBattle(thisRoom).AddPlayer(other.gameObject);
                
            }
            else if (other.gameObject.tag.Equals("Enemy"))
            {
                Battle_Manager.GetInstance().GetBattle(thisRoom).AddEnemy(other.gameObject);
            }
        }
        else
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                if (!thisRoom.IsExplored())
                {
                    thisRoom.SetExplored(true);
                    thisRoom.CloseDoors();
                    Battle_Manager.GetInstance().AddBattle(new Battle(thisRoom, other.gameObject));
                    StartCoroutine(Battle_Manager.GetInstance().GetBattle(thisRoom).SpawnEnemies());
                }
            }
        }
    }
}
