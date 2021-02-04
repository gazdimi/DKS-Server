using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Handle                                                             //server handle game input, data send from client
{
    public static void Welcome_Received(int fromClient, Packet packet)          //server receives welcome packet from client (handshake completed)
    {
        int client_id_received = packet.ReadInt();
        string username_received = packet.ReadString();
        Vector3 forward = packet.ReadVector3();
        Vector3 right = packet.ReadVector3();
        Console.WriteLine($"{Server.clients[client_id_received].tcp.socket.Client.RemoteEndPoint} connected with username {username_received} and id {client_id_received}");
        if (fromClient != client_id_received)
        {
            Console.WriteLine("Wrong client id...");
        }
        Server.clients[fromClient].SendToLobby(username_received, forward, right);  //after successful handshake place client (local player) in lobby             
        
        //Server.clients[fromClient].SendToGame(username_received);               //after successful handshake place client (local player) in game field
    }

    public static void PlayerMovement(int fromClient, Packet packet)            //extract info sent to server about player's movement
    {
        float[] inputs = new float[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadFloat();
        }
        //Quaternion rotation = packet.ReadQuaternion();
        Server.clients[fromClient].player.SetInput(inputs);//, rotation);           //send extracted info about player's movement (for specified client) to get handled
    }

    public static void Shooted(int fromClient, Packet packet) 
    {
        Vector3 shooting_direction = packet.ReadVector3();
        Server.clients[fromClient].player.Shoot(shooting_direction);
    }

    public static void StartGame(int fromClient, Packet packet)
    {
        SceneManager.LoadScene("DungeonScene");        
    }

    public static void PenValues(int fromClient, Packet packet)
    {
        Server.clients[fromClient].player.neuro = packet.ReadFloat();
        Server.clients[fromClient].player.extra = packet.ReadFloat();
        Server.clients[fromClient].player.certainty = packet.ReadFloat();
    }

    public static void PlayerHoldWeapon(int fromClient, Packet packet) 
    {
        int player_id = packet.ReadInt();                                       //the one holding the weapon
        float weapon_id = packet.ReadFloat();
        
        Server.clients[fromClient].player.ShowWeapon(weapon_id);
        Send.RemotePlayerWeapon(player_id, weapon_id);

    }
}
