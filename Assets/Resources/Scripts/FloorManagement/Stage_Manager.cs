using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Manager
{
    #region Singleton
    private static Stage_Manager instance =null;
    /// <summary>
    /// Get access to the only instance of the Stage Manager.
    /// </summary>
    /// <returns></returns>
    public static Stage_Manager GetInstance()
    {
        if (instance == null)
        {
            return new Stage_Manager();
        }
        return instance;
    }
    private Stage_Manager()
    {
        instance = this;
        startingRoomNumber = 10;
        maxRoomNumber = 50;
        floorNumber = 1;
        maxFloorNumber = 100;
    }
    #endregion

    private  int floorNumber;
    private  int maxFloorNumber;
    private  int maxRoomNumber;
    private  int startingRoomNumber;
    
    /// <summary>
    /// Gets the current floor number.
    /// </summary>
    /// <returns></returns>
    public int GetFloorNumber()
    {
        return floorNumber;
    }
    /// <summary>
    /// Gets the max floor number.
    /// </summary>
    /// <returns></returns>
    public int GetMaxFloorNumber()
    {
        return maxFloorNumber;
    }
    /// <summary>
    /// Calculates the next floor's room number to be generated.
    /// </summary>
    /// <returns></returns>
    public int GetNextFloorRoomNumber()
    {
        return ((int)Mathf.Floor(startingRoomNumber + (floorNumber *(maxRoomNumber-startingRoomNumber)/maxFloorNumber)));
    }
    public IEnumerator ChangeFloor()
    {
        //Change Floor
        floorNumber++;
        RandomnessMaestro.GetInstance().RefreshProbabilities();
        PlayerManager.GetInstance().DespawnPlayers();
        DungeonGenerator.GetInstance().ClearDungeon();
        yield return new WaitForSeconds(2);
        DungeonGenerator.GetInstance().CompleteDungeon(GetNextFloorRoomNumber());
        PlayerManager.GetInstance().SpawnPlayers();
    }
}
