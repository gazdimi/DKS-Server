using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocationManager
{
    /// <summary>
    /// Calculates and returns the appropriate location for the room to be placed.
    /// </summary>
    /// <param name="connectionSide"></param>
    /// <param name="opening_location"></param>
    /// <param name="new_opening_index"></param>
    /// <param name="TilesX"></param>
    /// <param name="TilesZ"></param>
    /// <returns></returns>
    public  static Vector3 GetApropriateLocationForRoom(string connectionSide,Vector3 opening_location,int new_opening_index,int TilesX,int TilesZ)
    {
        Vector3 new_placed_location = new Vector3(0, 0, 0);
        if (connectionSide == "Left")
        {
            new_placed_location.x = opening_location.x - (Tile.X_length * TilesX);
            new_placed_location.z = opening_location.z + (Tile.Z_length * (new_opening_index / TilesX));
        }
        else if (connectionSide == "Right")
        {
            new_placed_location.x = opening_location.x + Tile.X_length;
            new_placed_location.z = opening_location.z + (Tile.Z_length * (new_opening_index / TilesX));
        }
        else if (connectionSide == "Top")
        {
            new_placed_location.x = opening_location.x - (Tile.X_length * (new_opening_index % TilesX));
            new_placed_location.z = opening_location.z + (Tile.Z_length * TilesZ);
        }
        else
        {
            new_placed_location.x = opening_location.x - (Tile.X_length * (new_opening_index % TilesX));
            new_placed_location.z = opening_location.z - Tile.Z_length;
        }
        return new_placed_location;
    }
    /// <summary>
    /// finds and returns the location of the center of the room.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static Vector3 FindCenterOfRoom(IRoom room)
    {
        return new Vector3((room.RoomObject.transform.position.x + (room.RoomObject.transform.position.x + room.Tiles_number_x * Tile.X_length)) / 2f, 0, (room.RoomObject.transform.position.z + (room.RoomObject.transform.position.z - room.Tiles_number_z * Tile.Z_length)) / 2f);
    }
}
