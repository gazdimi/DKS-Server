using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    #region Singleton
    private static DataManager instance = null;

    public static DataManager GetInstance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            return new DataManager();
        }

    }
    private DataManager()
    {
        instance = this;
        Initialize_Type_Data();
    }
    #endregion
    /// <summary>
    /// Room helping struct.
    /// </summary>
    private struct Room_Type
    {
        public bool[] available_sides;
        public string room_type;
    }

    //Room Sizes Dictionary.
    private readonly Dictionary<string, (int, int)> room_sizes_data = new Dictionary<string, (int, int)>()
    {
        {"Small",(3,3) },
        {"Medium",(5,5) },
        {"Large",(7,7) }
    };

    //Room Available sides data.
    //HINT! Sides go in this order = [Left,Top,Right,Bottom] -> true if available, false if not available.
    private Room_Type[] room_types_data;

    //Corridor Available sides data.
    //HINT! Sides go in this order = [Left,Top,Right,Bottom] -> true if available, false if not available.
      Room_Type[] corridor_types_data;


    /// <summary>
    /// Initialized the data regarding the open sides of the rooms.
    /// </summary>
    public  void Initialize_Type_Data()
    {
        //Room type data initialization.
        room_types_data = new Room_Type[4];
        room_types_data[0] = new Room_Type() { available_sides = new bool[4] { true, true, true, true }, room_type = "SpawningRoom" };
        room_types_data[1] = new Room_Type() { available_sides = new bool[4] { true, true, true, true }, room_type = "FightingRoom" };
        room_types_data[2] = new Room_Type() { available_sides = new bool[4] { true, true, true, true }, room_type = "EndRoom" };
        room_types_data[3] = new Room_Type() { available_sides = new bool[4] { true, true, true, true }, room_type = "ChestRoom" };


        //Corridor type data initialization.
        corridor_types_data = new Room_Type[6];
        corridor_types_data[0] = new Room_Type() { available_sides = new bool[4] { true, false, true, false }, room_type = "Horizontal_Straight_Corridor" };
        corridor_types_data[1] = new Room_Type() { available_sides = new bool[4] { false, true, false, true }, room_type = "Vertical_Straight_Corridor" };
        corridor_types_data[2] = new Room_Type() { available_sides = new bool[4] { true, false, false, true }, room_type = "Bottom_Left_Corner_Corridor" };
        corridor_types_data[3] = new Room_Type() { available_sides = new bool[4] { false, false, true, true }, room_type = "Bottom_Right_Corner_Corridor" };
        corridor_types_data[4] = new Room_Type() { available_sides = new bool[4] { true, true, false, false }, room_type = "Top_Left_Corner_Corridor" };
        corridor_types_data[5] = new Room_Type() { available_sides = new bool[4] { false, true, true, false }, room_type = "Top_Right_Corner_Corridor" };

    }

    /// <summary>
    /// Gets all the available sides given the room type.
    /// </summary>
    /// <param name="roomType"></param>
    /// <returns></returns>
    public  List<string> GetRoomAvailableSides(string roomType)
    {
        List<string> aSides = new List<string>();
        foreach(Room_Type type in room_types_data)
        {
            if (type.room_type.Equals(roomType))
            {
                if (type.available_sides[0]) aSides.Add("Left");
                if (type.available_sides[1]) aSides.Add("Top");
                if (type.available_sides[2]) aSides.Add("Right");
                if (type.available_sides[3]) aSides.Add("Bottom");
                break;
            }
        }
        return aSides;

    }
    /// <summary>
    /// Gets all the available sides given the corridor type.
    /// </summary>
    /// <param name="roomType"></param>
    /// <returns></returns>
    public  List<string> GetCoridorAvailableSides(string corridorType)
    {
        List<string> aSides = new List<string>();
        foreach (Room_Type type in corridor_types_data)
        {
            if (type.room_type.Equals(corridorType))
            {
                if (type.available_sides[0]) aSides.Add("Left");
                if (type.available_sides[1]) aSides.Add("Top");
                if (type.available_sides[2]) aSides.Add("Right");
                if (type.available_sides[3]) aSides.Add("Bottom");
                break;
            }
        }
        return aSides;

    }

    /// <summary>
    /// Get room proportions depending on room size.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public  (int,int) Search_Sizes_Dictionary(string size)
    {
        return room_sizes_data[size];
    }

    /// <summary>
    /// Get correct adjacent rooms depending on the side.
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    public  List<string> Search_Available_Sides(string side,string type)
    {
        int index;
        if (side == "Left")
        {
            index = 0;
        }
        else if (side == "Top")
        {
            index = 1;
        }
        else if (side == "Right")
        {
            index = 2;
        }
        else 
        {
            index = 3;
        }
        List<string> correct_rooms = new List<string>();
        if (type == "Room")
        {
            foreach (Room_Type rm in room_types_data)
            {
                if (rm.available_sides[index] == true)
                {
                    if (!rm.room_type.Equals("SpawningRoom"))
                    {
                        correct_rooms.Add(rm.room_type);
                    }
                }
            }
        }
        else
        {

            foreach (Room_Type rm in corridor_types_data)
            {
                if (rm.available_sides[index] == true)
                {

                    correct_rooms.Add(rm.room_type);
                }
            }
        }
        return correct_rooms;
    }

    /// <summary>
    /// Returns a smaller size of the room depending on times_smaller
    /// </summary>
    /// <param name="oldsize"></param>
    /// <param name="times_smaller"></param>
    /// <returns></returns>
    //Ex oldsize="Large" if times_smaller = 1 will return "Medium" / if times_smaller = 2 will return "Small".
    public  (string,int,int) ReturnSmallerSize(string oldsize,int times_smaller)
    {
        string current_size = oldsize;
        while (times_smaller > 0)
        {
            switch (current_size)
            {
                case "Large":
                    current_size = "Medium";
                    break;
                case "Medium":
                    current_size = "Small";
                    break;
                default:
                    current_size = "Small";
                    break;
            }
            times_smaller--;
        }
        (int x, int z) = Search_Sizes_Dictionary(current_size);
        return (current_size,x,z);
    }
}
