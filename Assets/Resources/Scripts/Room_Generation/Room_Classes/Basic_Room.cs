using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Basic_Room : IRoom
{
    public GameObject RoomObject { get; set; }
    public Vector3 Position { get; set; }
    public string Category { get; set; }
    public string Type { get; set; }
    public int Tiles_number_x { get; set; }
    public int Tiles_number_z { get; set; }
    public List<Tile> RoomTiles { get; set; }
    public List<GameObject> Instantiated_Tiles { get; set; }
    public List<string> Available_Sides { get; set; }
    public IRoom AdjRoomTop { get; set; }
    public IRoom AdjRoomLeft { get; set; }
    public IRoom AdjRoomRight { get; set; }
    public IRoom AdjRoomBottom { get; set; }
    protected bool explored;

    public Basic_Room(List<GameObject> tiles, string type, int tiles_x, int tiles_z)
    {
        this.Category = "Room";
        this.RoomTiles = new List<Tile>();
        this.Type = type;
        this.Tiles_number_x = tiles_x;
        this.Tiles_number_z = tiles_z;
        explored = false;
        CreateRoom(tiles);
    }
    /// <summary>
    /// Checks if the room is explored.
    /// </summary>
    /// <returns></returns>
    public bool IsExplored()
    {
        return explored;
    }
    /// <summary>
    /// Sets the room explored variable.
    /// </summary>
    /// <param name="exp"></param>
    public void SetExplored(bool exp)
    {
        explored = exp;
    }
    public abstract void CreateRoom(List<GameObject> tiles);
    /// <summary>
    /// Calculates the index of the expected opening depending on the side.
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    public int CalculateOpening(string side)
    {
        int index_x, index_z;
        if (side == "Left")
        {
            index_z = this.Tiles_number_z / 2;
            index_x = 0;


        }
        else if (side == "Top")
        {
            index_z = 0;
            index_x = this.Tiles_number_x / 2;
        }
        else if (side == "Right")
        {
            index_z = this.Tiles_number_z / 2;
            index_x = this.Tiles_number_x - 1;
        }
        else
        {
            index_z = this.Tiles_number_z - 1;
            index_x = this.Tiles_number_x / 2;
        }
        return (index_z * Tiles_number_x + index_x);
    }
    /// <summary>
    /// Create new opening to connect rooms on selected side.
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    public Vector3 CreateOpening(int indexopening,string side)
    {
        Tile t;
        //Tile to be placed.
        if (side == "Top")
        {
             t = new Tile("Top_Door", PrefabManager.GetInstance().GetAllRoomTiles().Where(obj => obj.name == "Top_Door").First(), RoomTiles[indexopening].Position);
        }
        else if (side == "Right")
        {
            t = new Tile("Right_Door", PrefabManager.GetInstance().GetAllRoomTiles().Where(obj => obj.name == "Right_Door").First(), RoomTiles[indexopening].Position);
        }
        else if (side == "Bottom")
        {
            t = new Tile("Bottom_Door", PrefabManager.GetInstance().GetAllRoomTiles().Where(obj => obj.name == "Bottom_Door").First(), RoomTiles[indexopening].Position);
        }
        else
        {
            t = new Tile("Left_Door", PrefabManager.GetInstance().GetAllRoomTiles().Where(obj => obj.name == "Left_Door").First(), RoomTiles[indexopening].Position);
        }

        //Replace opening.
        RoomTiles[indexopening] = t;
        Vector3 oldtileloc = new Vector3(0, 0, 0);
        if (Instantiated_Tiles!=null)//If room is already instatiated.
        {
            oldtileloc = Instantiated_Tiles[indexopening].transform.position;
            //Destroy old opening.
            Object.Destroy(Instantiated_Tiles[indexopening]);
            //Instantiate new opening.
            Instantiated_Tiles[indexopening] = Object.Instantiate(t.Objtile, oldtileloc, new Quaternion(), RoomObject.transform);
        }
        Available_Sides.Remove(side);
        return oldtileloc;

    }
    /// <summary>
    /// Create an adjacent room on selected side.
    /// </summary>
    /// <param name="side"></param>
    /// <param name="opening_location"></param>
    /// <returns></returns>
    public (IRoom,Vector3) CreateAdjacentRoom()
    {
        //Chose if next room is gonna be corridor or room.
        string room_corridor = RandomnessMaestro.GetInstance().Choose_Room_Or_Corridor();
        List<string> available_rooms = new List<string>();
        //Get all the available rooms based on the available sides of the current room.
        foreach (string availableSide in Available_Sides)
        {
            available_rooms.AddRange(ValidationMaestro.GetAppropriateRooms(room_corridor, GetAdjacentSide(availableSide)));
        }
        available_rooms = available_rooms.Distinct().ToList();//Remove duplicates.
        if (room_corridor == "Room")
        {
            string roomsize = RandomnessMaestro.GetInstance().Choose_Room_Size();//Select the room size.
            (int sizex, int sizez) = DataManager.GetInstance().Search_Sizes_Dictionary(roomsize);//Get the room size data.
            IRoom new_room = RoomFactory.Build(RandomnessMaestro.GetInstance().Choose_Room_Type(available_rooms), PrefabManager.GetInstance().GetAllRoomTiles(), sizex, sizez);//Construct the new room.
            string connectionSide="";
            foreach(string avside in Available_Sides)
            {
                if (new_room.Available_Sides.Contains(GetAdjacentSide(avside)))
                {
                    connectionSide = avside;
                    break;
                }
            }//Find the connection side between the current room and the new one.
            string adjside = GetAdjacentSide(connectionSide);//Get the side that the new room needs to have available.
            int openindex = CalculateOpening(connectionSide);
            Vector3 opening_location = Instantiated_Tiles[openindex].transform.position;//Location of the tile that the opening is gonna be.
            int new_opening_index = new_room.CalculateOpening(adjside);//Calculate the opening of the new room.
            //Place new adjacent room appropriately.
            Vector3 new_placed_location = LocationManager.GetApropriateLocationForRoom(connectionSide, opening_location, new_opening_index, new_room.Tiles_number_x, new_room.Tiles_number_z);
            bool end = false;
            //While location is taken by another object.
            while (!ValidationMaestro.IsNotClaimed(new_placed_location, new_room.Tiles_number_x, new_room.Tiles_number_z)&&!end)
            {
                //If the smallest room doesn't fit, stop.
                if (roomsize == "Small")
                {
                    end = true;
                }
                //Return a smaller size.
                (roomsize, sizex, sizez) = DataManager.GetInstance().ReturnSmallerSize(roomsize, 1);
                //Re-Construct the room.
                new_room = RoomFactory.Build(RandomnessMaestro.GetInstance().Choose_Room_Type(available_rooms), PrefabManager.GetInstance().GetAllRoomTiles(), sizex, sizez);
                //Re-Calculate opening.
                new_opening_index = new_room.CalculateOpening(adjside);
                //Re-Place the room appropriately.
                new_placed_location = LocationManager.GetApropriateLocationForRoom(connectionSide, opening_location, new_opening_index, new_room.Tiles_number_x, new_room.Tiles_number_z);
            }
            if (end)//if smaller doesn't fit.
            {
                this.Available_Sides.Remove(connectionSide);
                return (null, new Vector3(0, 0, 0));
            }
            if (new_room.Type.Equals("EndRoom"))//Check if the floor end room is placed.
            {
                new_room.Available_Sides.Clear();
                RandomnessMaestro.GetInstance().endRoomPlaced = true;
                Debug.LogError("Placed end room at: " + DungeonGenerator.GetInstance().roomsPlaced);
            }
            //If everything is okay, place the room normally.
            this.Available_Sides.Remove(connectionSide);
            new_room.Available_Sides.Remove(adjside);
            if(this.Category.Equals("Room"))CreateOpening(openindex, connectionSide);//Create opening if this is a room.
            Vector3 newopenloc =new_room.CreateOpening(new_opening_index, adjside); //Create opening to the adjacent room.
            //Make this room parent of the new one.
            if (this.Category == "Room")
            {
                switch (connectionSide)
                {
                    case "Left":
                        this.AdjRoomLeft = new_room;
                        break;
                    case "Top":
                        this.AdjRoomTop = new_room;
                        break;
                    case "Right":
                        this.AdjRoomRight = new_room;
                        break;
                    case "Bottom":
                        this.AdjRoomBottom = new_room;
                        break;
                }
            }
            else
            {
                ((Basic_Corridor)this).Child = new_room;
            }
            switch (adjside)
            {
                case "Left":
                    ((Basic_Room)new_room).AdjRoomLeft = this;
                    break;
                case "Top":
                    ((Basic_Room)new_room).AdjRoomTop = this;
                    break;
                case "Right":
                    ((Basic_Room)new_room).AdjRoomRight = this;
                    break;
                case "Bottom":
                    ((Basic_Room)new_room).AdjRoomBottom = this;
                    break;
            }
            return (new_room, new_placed_location);
        }
        else
        {
            //Choose randomly the type of corridor.
            string corridor_chosen_type = RandomnessMaestro.GetInstance().Choose_Corridor_Type();
            //Choose only corridors that match the type
            List<string> proper_rooms = available_rooms.Where(x => x.Contains(corridor_chosen_type)).ToList();
            IRoom new_room = RoomFactory.Build(proper_rooms[Random.Range(0,proper_rooms.Count-1)], PrefabManager.GetInstance().GetAllCorridorTiles(), 1, 1);
            string connectionSide = "";
            foreach(string avside in Available_Sides)
            {
                if (new_room.Available_Sides.Contains(GetAdjacentSide(avside)))
                {
                    connectionSide = avside;
                    break;
                }
            }
            string adjside = GetAdjacentSide(connectionSide);
            int openindex = CalculateOpening(connectionSide);
            Vector3 opening_location = Instantiated_Tiles[openindex].transform.position;
            //Calculate the opening of the new room.
            int new_opening_index = new_room.CalculateOpening(adjside);
            //Place new adjacent room appropriately.
            Vector3 new_placed_location = LocationManager.GetApropriateLocationForRoom(connectionSide, opening_location, new_opening_index, new_room.Tiles_number_x, new_room.Tiles_number_z);
            //If location is taken by another object.
            if (!ValidationMaestro.IsNotClaimed(new_placed_location, new_room.Tiles_number_x, new_room.Tiles_number_z))
            {
                this.Available_Sides.Remove(connectionSide);
                return (null, new Vector3(0, 0, 0));
            }
            else
            {
                if (this.Category.Equals("Room")) CreateOpening(openindex, connectionSide);
                this.Available_Sides.Remove(connectionSide);
                new_room.Available_Sides.Remove(adjside);
                if (this.Category == "Room")
                {
                    switch (connectionSide)
                    {
                        case "Left":
                            this.AdjRoomLeft = new_room;
                            break;
                        case "Top":
                            this.AdjRoomTop = new_room;
                            break;
                        case "Right":
                            this.AdjRoomRight = new_room;
                            break;
                        case "Bottom":
                            this.AdjRoomBottom = new_room;
                            break;
                    }
                }
                else
                {
                    ((Basic_Corridor)this).Child = new_room;
                }
                
                ((Basic_Corridor)new_room).Parent = this; 
                return (new_room, new_placed_location);
            }
        }

    }
    /// <summary>
    /// Detects and seals the opening with a wall depending on the side.
    /// </summary>
    /// <param name="side"></param>
    public void CloseOpening(string side)
    {
        Tile newtile;
        int openingindex = CalculateOpening(side);
        if (side == "Left")
        { 
            
            newtile = new Tile("Left_Wall", PrefabManager.GetInstance().GetAllRoomTiles().Where(obj => obj.name == "Left_Wall").First(), RoomTiles[openingindex].Position);
            
        }
        else if (side == "Top")
        {
            newtile = new Tile("Top_Wall", PrefabManager.GetInstance().GetAllRoomTiles().Where(obj => obj.name == "Top_Wall").First(), RoomTiles[openingindex].Position);
        }
        else if (side == "Right")
        {
            newtile = new Tile("Right_Wall", PrefabManager.GetInstance().GetAllRoomTiles().Where(obj => obj.name == "Right_Wall").First(), RoomTiles[openingindex].Position);
        }
        else
        {
            newtile = new Tile("Bottom_Wall", PrefabManager.GetInstance().GetAllRoomTiles().Where(obj => obj.name == "Bottom_Wall").First(), RoomTiles[openingindex].Position);
        }
        RoomTiles[openingindex] = newtile;
        Vector3 oldtileloc = new Vector3(0, 0, 0);

        if (Instantiated_Tiles != null)//If room is already instatiated.
        {
            oldtileloc = Instantiated_Tiles[openingindex].transform.position;
            //Destroy old opening.
            Object.Destroy(Instantiated_Tiles[openingindex]);
            //Instantiate new opening.
            Instantiated_Tiles[openingindex] = Object.Instantiate(newtile.Objtile, oldtileloc, new Quaternion(), RoomObject.transform);
        }
    }
    /// <summary>
    /// Returns the adjacent side of choosen side.
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    string GetAdjacentSide(string side)
    {
        if (side == "Left")
            return "Right";
        else if (side == "Right")
            return "Left";
        else if (side == "Top")
            return "Bottom";
        else
            return "Top";
    }
    /// <summary>
    /// Closes all the doors of the room.
    /// </summary>
    public void CloseDoors()
    {
        if (AdjRoomTop != null)
        {
            int doorindex = CalculateOpening("Top");
            Instantiated_Tiles[doorindex].GetComponent<Animator>().SetBool("isOpen", false);
        }
        if (AdjRoomLeft != null)
        {
            int doorindex = CalculateOpening("Left");
            Instantiated_Tiles[doorindex].GetComponent<Animator>().SetBool("isOpen", false);
        }
        if (AdjRoomRight != null)
        {
            int doorindex = CalculateOpening("Right");
            Instantiated_Tiles[doorindex].GetComponent<Animator>().SetBool("isOpen", false);
        }
        if (AdjRoomBottom != null)
        {
            int doorindex = CalculateOpening("Bottom");
            Instantiated_Tiles[doorindex].GetComponent<Animator>().SetBool("isOpen", false);
        }
    }
    /// <summary>
    /// Opens all the doors of the room.
    /// </summary>
    public void OpenDoors()
    {
        if (AdjRoomTop != null)
        {
            int doorindex = CalculateOpening("Top");
            Instantiated_Tiles[doorindex].GetComponent<Animator>().SetBool("isOpen", true);
        }
        if (AdjRoomLeft != null)
        {
            int doorindex = CalculateOpening("Left");
            Instantiated_Tiles[doorindex].GetComponent<Animator>().SetBool("isOpen", true);
        }
        if (AdjRoomRight != null)
        {
            int doorindex = CalculateOpening("Right");
            Instantiated_Tiles[doorindex].GetComponent<Animator>().SetBool("isOpen", true);
        }
        if (AdjRoomBottom != null)
        {
            int doorindex = CalculateOpening("Bottom");
            Instantiated_Tiles[doorindex].GetComponent<Animator>().SetBool("isOpen", true);
        }

    }
}
