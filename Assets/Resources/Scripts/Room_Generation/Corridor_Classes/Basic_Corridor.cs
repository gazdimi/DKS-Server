using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Basic_Corridor : Basic_Room
{
    public IRoom Parent { get; set; }
    public IRoom Child { get; set; }

    public Basic_Corridor(List<GameObject> tiles, string type, int tiles_x, int tiles_z):base(tiles,type,tiles_x,tiles_z)
    {
        Category = "Corridor";
        this.RoomTiles = new List<Tile>();
        this.Type = type;
        this.Tiles_number_x = tiles_x;
        this.Tiles_number_z = tiles_z;
        CreateRoom(tiles);
    }
}
