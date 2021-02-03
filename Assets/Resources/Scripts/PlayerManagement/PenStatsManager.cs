using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenStatsManager
{
    #region Singleton
    private static PenStatsManager instance = null;
    public static PenStatsManager GetInstance()
    {
        if (instance == null)
        {
            return new PenStatsManager();
        }
        return instance;
    }
    PenStatsManager()
    {
        instance = this;
    }
    #endregion
    /// <summary>
    /// Get all player pen stats.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public (float,float,float) GetPenStats(GameObject player)
    {
        return (player.GetComponent<CharacterBehaviour>().pen.GetNeurotism(),
            player.GetComponent<CharacterBehaviour>().pen.GetExtraversion(),
            player.GetComponent<CharacterBehaviour>().pen.GetCertainty());
    }
    /// <summary>
    /// Get the arithmetic mean pen characteristics from selected player's list.
    /// </summary>
    /// <param name="players"></param>
    /// <returns></returns>
    public (float Neurotisism,float Extraversion,float Certainty) GetMeanPen(List<GameObject> players)
    {
        float neuro = 0;
        float extra = 0;
        float certain = 0;
        foreach(GameObject player in players)
        {
            neuro += player.GetComponent<CharacterBehaviour>().pen.GetNeurotism();
            extra += player.GetComponent<CharacterBehaviour>().pen.GetExtraversion();
            certain += player.GetComponent<CharacterBehaviour>().pen.GetCertainty();
        }
        return (neuro / players.Count, extra / players.Count, certain / players.Count);   
    }
}
