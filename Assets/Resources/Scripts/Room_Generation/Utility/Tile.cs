using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
   public GameObject Objtile;
    public static int X_length = 10;
    public static int Z_length = 10;
   public string Type { get; set; }
   public Vector3 Position { get; set; }
        

   public Tile(string type,GameObject objtile,Vector3 position)
    {
        this.Objtile = objtile;
        this.Type = type;
        this.Position = position;
    }
}
