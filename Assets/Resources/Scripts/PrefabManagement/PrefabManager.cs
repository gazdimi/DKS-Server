using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager
{
    #region Singleton
    private static PrefabManager instance = null;

    public static PrefabManager GetInstance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            return new PrefabManager();
        }

    }
    private PrefabManager()
    {
        instance = this;
        LoadPrefabs();
    }
    #endregion
     private List<GameObject> allroomtiles;
     private List<GameObject> allcorridortiles;

    /// <summary>
    /// Returns all loaded tiles from project.
    /// </summary>
    /// <returns></returns>
    public  List<GameObject> GetAllRoomTiles()
    {
        return allroomtiles;
    }

    /// <summary>
    /// Returns all loaded corridors from project.
    /// </summary>
    /// <returns></returns>
    public  List<GameObject> GetAllCorridorTiles()
    {
        return allcorridortiles;
    }
    /// <summary>
    /// Loads all prefabs from project.
    /// </summary>
    /// <returns></returns>
    public  bool LoadPrefabs()
    {
        try
        {
            allroomtiles = new List<GameObject>((Resources.LoadAll<GameObject>("Prefabs/Room_Generation/Tiles/Room_Tiles"))); // Load all tile prefabs from the project folder.
            allcorridortiles = new List<GameObject>((Resources.LoadAll<GameObject>("Prefabs/Room_Generation/Tiles/Corridor_Tiles"))); // Load all corridor prefabs from the project folder.
            return true;
        }
        catch
        {
            return false;
        }
    }
}
