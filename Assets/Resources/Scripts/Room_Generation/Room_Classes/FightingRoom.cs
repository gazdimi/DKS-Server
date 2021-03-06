﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FightingRoom : Basic_Room
{
   
    public FightingRoom(List<GameObject> tiles, string type, int tiles_x, int tiles_z) : base(tiles, type, tiles_x, tiles_z)
    {
        Available_Sides = DataManager.GetInstance().GetRoomAvailableSides(type);
    }

    public override void CreateRoom(List<GameObject> tiles)
    {
        Tile newtile;
        float xpos = Position.x, ypos = Position.y, zpos = Position.z;
        for (int i = 0; i < Tiles_number_x; i++)
        {
            for (int j = 0; j < Tiles_number_z; j++)
            {
                if (i == 0 && j == 0)
                {
                    newtile = new Tile("Left_Top_Corner", tiles.Where(obj => obj.name == "Left_Top_Corner").First(), new Vector3(xpos, ypos, zpos));
                }
                else if (i == 0 && j == Tiles_number_z - 1)
                {
                    newtile = new Tile("Right_Top_Corner", tiles.Where(obj => obj.name == "Right_Top_Corner").First(), new Vector3(xpos, ypos, zpos));
                }
                else if (i == Tiles_number_x - 1 && j == 0)
                {
                    newtile = new Tile("Left_Bottom_Corner", tiles.Where(obj => obj.name == "Left_Bottom_Corner").First(), new Vector3(xpos, ypos, zpos));
                }
                else if (i == Tiles_number_x - 1 && j == Tiles_number_z - 1)
                {
                    newtile = new Tile("Right_Bottom_Corner", tiles.Where(obj => obj.name == "Right_Bottom_Corner").First(), new Vector3(xpos, ypos, zpos));
                }
                else if (i == 0)
                {
                    newtile = new Tile("Top_Wall", tiles.Where(obj => obj.name == "Top_Wall").First(), new Vector3(xpos, ypos, zpos));
                }
                else if (i == Tiles_number_x - 1)
                {
                    newtile = new Tile("Bottom_Wall", tiles.Where(obj => obj.name == "Bottom_Wall").First(), new Vector3(xpos, ypos, zpos));
                }
                else if (j == 0)
                {
                    newtile = new Tile("Left_Wall", tiles.Where(obj => obj.name == "Left_Wall").First(), new Vector3(xpos, ypos, zpos));
                }
                else if (j == Tiles_number_z - 1)
                {
                    newtile = new Tile("Right_Wall", tiles.Where(obj => obj.name == "Right_Wall").First(), new Vector3(xpos, ypos, zpos));
                }
                else
                {
                    newtile = new Tile("Center", tiles.Where(obj => obj.name == "Center").First(), new Vector3(xpos, ypos, zpos));
                }

                RoomTiles.Add(newtile);

                xpos += Tile.X_length;

            }
            xpos = Position.x;
            zpos -= Tile.Z_length;
        }

    }
}
