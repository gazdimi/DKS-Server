using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty_Manager
{
    #region Singleton
    private static Difficulty_Manager instance = null;
    public static Difficulty_Manager GetInstance()
    {
        if (instance == null)
        {
            return new Difficulty_Manager();
        }
        return instance;
    }
    private Difficulty_Manager()
    {
        instance = this;
        CalculateNewDifficultyFactor();
    }
    #endregion

    private  float difficultyFactor;
    private  int playerMeanMastery = 10;//TODO implement a player leveling system.

    /// <summary>
    /// Calculates the floor's difficulty factor depending on the floor number, player mastery and a standard factor.
    /// </summary>
    public  void CalculateNewDifficultyFactor()
    {
        float floorNumberPercent = 30f;
        float playerMasteryPercent =60f;
        float standardPercent = 10f;
        difficultyFactor = (Stage_Manager.GetInstance().GetFloorNumber() / Stage_Manager.GetInstance().GetMaxFloorNumber() * floorNumberPercent) + (playerMeanMastery / 100 * playerMasteryPercent) + standardPercent;
    }
    /// <summary>
    /// Returns the difficulty factor of the floor;
    /// </summary>
    /// <returns></returns>
    public float GetDifficultyFactor()
    {
        return difficultyFactor;
    }

}
