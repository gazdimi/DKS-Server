using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomnessMaestro
{
    #region Singleton
    private static RandomnessMaestro instance = null;

    public static RandomnessMaestro GetInstance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            return new RandomnessMaestro();
        }

    }
    private RandomnessMaestro()
    {
        instance = this;
        RefreshProbabilities();
    }
    #endregion
    public bool endRoomPlaced = false;

    public void RefreshProbabilities()
    {
        room_corridor_chance = new List<Possibility>{
        new Possibility("Corridor",60f),
        new Possibility("Room",40f) };
        corridor_type = new List<Possibility>{
        new Possibility("Corner",30f),
        new Possibility("Straight",70f) };
        room_size = new List<Possibility>{
        new Possibility("Small",20f),
        new Possibility("Medium",50f),
        new Possibility("Large",30f)
        };
        room_type = new List<Possibility>{
        new Possibility("FightingRoom",98.9f),
        new Possibility("ChestRoom",1f),
        new Possibility("EndRoom",0.1f)
        };
        endRoomPlaced = false;
    }
    //Room or Corridor chances.
    private List<Possibility> room_corridor_chance;
    //Corridor type chances.
    private List<Possibility> corridor_type;
    //Room size chances.
    private List<Possibility> room_size;

    //Room type chances.
    private List<Possibility> room_type;


    /// <summary>
    /// Shows appropriate error (debug perposes)
    /// </summary>
    /// <param name="error"></param>
    private  void ShowError(string error)
    {
        switch (error)
        {
            case "NO_SIDES":
                Debug.LogError("No available sides for the room");
                break;
        }
    }
    /// <summary>
    /// Chooses an available open side.
    /// </summary>
    /// <param name="room"></param>
    /// <returns>Random available side</returns>
    public  string OpenRandomAvailableSide(IRoom room)
    {
        if (room.Available_Sides.Count > 0)
        {
            return room.Available_Sides[Random.Range(0, room.Available_Sides.Count - 1)];
        }
        else
        {
            ShowError("NO_SIDES");
            return null;
        }
    }
    /// <summary>
    /// Decides if next room is corridor or room.
    /// </summary>
    /// <returns></returns>
    public  string Choose_Room_Or_Corridor()
    {
        return RandomProbability.Choose(room_corridor_chance);
    }
    /// <summary>
    /// Decides the next room size.
    /// </summary>
    /// <returns></returns>
    public  string Choose_Room_Size()
    {
        return RandomProbability.Choose(room_size);
    }
    /// <summary>
    /// Decides the next room type depending on the list of the available rooms.
    /// </summary>
    /// <param name="available_rooms"></param>
    /// <returns></returns>
    public  string Choose_Room_Type(List<string> available_rooms)
    {
        List<Possibility> appropriateProbabilities = CalculateProbabilites(available_rooms);
        return RandomProbability.Choose(appropriateProbabilities);
    }
    /// <summary>
    /// Decides the corridor type.
    /// </summary>
    /// <returns></returns>
    public  string Choose_Corridor_Type()
    {
        return RandomProbability.Choose(corridor_type);
    }
    /// <summary>
    /// Calculates the probabibilites of all the available rooms depending on some formulas.
    /// </summary>
    /// <param name="available_rooms"></param>
    /// <returns></returns>
     List<Possibility> CalculateProbabilites(List<string>available_rooms)
     { 
        List<Possibility> appropriateProbabilities = new List<Possibility> ();
        foreach (string room in available_rooms)
        {
            appropriateProbabilities.Add(room_type.Find(x => x.GetItem() == room));//Get only the available rooms probabilities.
        }
        float endRoomProbability;
        float endRoomStartingProb = room_type.Find(x => x.GetItem() == "EndRoom").GetValue();
        float t;
        float probabilityNotEndroom;
        if (!endRoomPlaced)
        {
            /*Variable to decide if the room has more probabilities to spawn the start, middle or the end of the dungeon.
             * Low --------> High Power = Start --------> End.
            */
            t = (float)System.Math.Pow((float)(DungeonGenerator.GetInstance().roomsPlaced + 1) / DungeonGenerator.GetInstance().RoomNumber, 10);
            endRoomProbability = endRoomStartingProb * (1 - t) + 100 * t;
            probabilityNotEndroom = 100 - endRoomProbability;
        }
        else
        {
            endRoomProbability = 0;
            probabilityNotEndroom = 100;
        }
        for (int i = 0; i < appropriateProbabilities.Count; i++)
        {
            if (appropriateProbabilities[i].GetItem().Equals("EndRoom"))
            {
                appropriateProbabilities[i].SetValue(endRoomProbability);
            }
            else
            {
                appropriateProbabilities[i].SetValue(appropriateProbabilities[i].GetValue() * probabilityNotEndroom / (100 - endRoomStartingProb));
            }
        }
        return appropriateProbabilities;
    }
}
