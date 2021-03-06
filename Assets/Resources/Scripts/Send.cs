﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Send                                                       //preparing packets to send from server to client
{
    public static void Welcome(int toClient, string message)            //which client to send the packet, message to be sent (server sends welcome packet for handshake with the client)
    {
        using (Packet packet = new Packet((int)ServerPackets.welcome))  //create the welcome packet (using statement ensures the correct use of IDisposable objects)
        {
            packet.Write(message);                                      //fill with data (write a message and client's id)
            packet.Write(toClient);

            SendTcpData(toClient, packet);                              //sent packet to specified client via tcp
        }
    }
    //----------------------------------------------send TCP data--------------------------------------------
    private static void SendTcpData(int toClient, Packet packet)        //prepare packet to be sent to specified client via tcp
    {
        packet.WriteLength();                                           //take packet's content length and put it at the beginning of the packet (buffer)
        Server.clients[toClient].tcp.SendData(packet);                  //send data from server to the specified client
    }

    private static void SendTcpDataToAll(Packet packet)                 //send data-packet to all connected-remote clients via tcp
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.maximum_players; i++)
        {
            Server.clients[i].tcp.SendData(packet);                     //send packet to each client (using tcp)
        }
    }

    private static void SendTcpDataToAll(int excepted_client, Packet packet) //send data-packet to all remote clients except one specified client via tcp
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.maximum_players; i++)
        {
            if (i != excepted_client)                                     //check for the specified excepted client
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }
    }

    //----------------------------------------------send UDP data--------------------------------------------
    private static void SendUdpData(int to_client, Packet packet)       //prepare packet to be sent
    {
        packet.WriteLength();
        Server.clients[to_client].udp.SendData(packet);
    }

    private static void SendUdpDataToAll(Packet packet)                 //send data-packet to all remote clients (connected) via udp
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.maximum_players; i++)
        {
            Server.clients[i].udp.SendData(packet);                     //send packet to each client using udp
        }
    }

    private static void SendUdpDataToAll(int exceptClient, Packet packet) //send data-packet to all remote clients except specified one via udp
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.maximum_players; i++)
        {
            if (i != exceptClient)                                      //check for the specific client
            {
                Server.clients[i].udp.SendData(packet);
            }
        }
    }

    //---------------------------------------------player initialization and movement------------------------
    public static void Generate(int toClient, ServerPlayer player)
    {
        using (Packet packet = new Packet((int)ServerPackets.generated_player))     //after generating player send him a test packet
        {
            packet.Write(player.player_id);
            packet.Write(player.username);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            SendTcpData(toClient, packet);
        }
    }

    public static void PlayerPosition(ServerPlayer player)                        //inform all players about a player's position
    {
        using (Packet packet = new Packet((int)ServerPackets.player_position))
        {
            packet.Write(player.player_id);
            packet.Write(player.transform.position);                        //write to the packet player's new position
            SendUdpDataToAll(packet);
        }
    }

    public static void PlayerRotation(ServerPlayer player)                        //inform all players about a player's rotation
    {
        using (Packet packet = new Packet((int)ServerPackets.player_rotation))
        {
            packet.Write(player.player_id);
            packet.Write(player.transform.forward);
            SendUdpDataToAll(player.player_id, packet);                     //inform everyone except the player that has been rotating
        }
    }

    public static void DisconnectPlayer(int player_id) 
    {
        using (Packet packet = new Packet((int)ServerPackets.disconnected_player))  //create data-packet with client's id (player's) inside that is disconnected
        {
            packet.Write(player_id);
            SendTcpDataToAll(packet);                                       //inform all other connected players that another player has disconnected
        }
    }

    public static void PlayerHealth(ServerPlayer player)                          //inform all players if someone's health has changed
    {
        using (Packet packet = new Packet((int)ServerPackets.player_health)) 
        {
            packet.Write(player.player_id);                                 //add client's id inside the packet (whose health has changed)
            packet.Write(player.current_health);
            SendTcpDataToAll(packet);                                       //send packet to all remote clients via tcp
        }
    }

    public static void RegeneratePlayer(ServerPlayer player) 
    {
        using (Packet packet = new Packet((int)ServerPackets.regenerated_player)) 
        {
            packet.Write(player.player_id);
            SendTcpDataToAll(packet);                                       //inform all players that the dead one has been regenerated via tcp
        }
    }

    public static void GenerateDungeon() 
    {
        
        DungeonGenerator.GetInstance().CompleteDungeon(Stage_Manager.GetInstance().GetNextFloorRoomNumber());
        List<IRoom> rooms = DungeonGenerator.GetInstance().allrooms;
        foreach (IRoom room in rooms)
        {
            using (Packet packet = new Packet((int)ServerPackets.generate_IRoom))
            {

                packet.Write(room.RoomObject.name);
                packet.Write(room.RoomObject.transform.position);
                packet.Write(room.Tiles_number_x);
                packet.Write(room.Tiles_number_z);
                packet.Write(room.Category);
                packet.Write(room.Type);
                SendTcpDataToAll(packet);
            }

            foreach (GameObject tile in room.Instantiated_Tiles) 
            {
                string tile_name = tile.name.Replace("(Clone)","");
                Vector3 tile_position = tile.transform.position;
                Quaternion tile_rotation = tile.transform.rotation;
                using (Packet packet = new Packet((int)ServerPackets.generate_Tile))
                {
                    packet.Write(tile_name);
                    packet.Write(tile_position);
                    packet.Write(tile_rotation);
                    SendTcpDataToAll(packet);
                }
            }

        }
    }

    public static void Teleport(ServerPlayer player)                              //inform all players about a player's position
    {
        using (Packet packet = new Packet((int)ServerPackets.player_position))
        {
            packet.Write(player.player_id);
            packet.Write(player.transform.position);                        //write to the packet player's new position
            SendTcpDataToAll(packet);
        }
    }

    public static void AskPen()
    {
        using (Packet packet = new Packet((int)ServerPackets.askPen))
        {
            SendTcpDataToAll(packet);
        }
    }
    public static void RemoteDoors(Vector3 doorLoc,bool isopen)
    {
        using (Packet packet = new Packet((int)ServerPackets.remoteDoor))
        {
            packet.Write(doorLoc);
            packet.Write(isopen);
            SendTcpDataToAll(packet);
        }
    }

    public static void WeaponLocation(string name, Vector3 location, float id)
    {
        using (Packet packet = new Packet((int)ServerPackets.weaponLocation))
        {
            packet.Write(name);
            packet.Write(location);
            packet.Write(id);
            SendTcpDataToAll(packet);
        }
    }

    public static void RemotePlayerWeapon(int player_id, float weapon_id) {
        using (Packet packet = new Packet((int)ServerPackets.remotePlayerWeapon)) 
        {
            packet.Write(player_id);
            packet.Write(weapon_id);
            SendTcpDataToAll(player_id, packet);
        }
    }

    public static void SpawnEnemy(string name, Vector3 location)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            packet.Write(name);
            packet.Write(location);
            SendTcpDataToAll(packet);
        }
    }
    public static void SpawnMod(string modname, int enemyid)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnMod))
        {
            packet.Write(modname);
            packet.Write(enemyid);
            SendTcpDataToAll(packet);
        }
    }
    public static void MoveEnemy(Vector3 enemyLoc, Quaternion enemyRot,int id)
    {
        using (Packet packet = new Packet((int)ServerPackets.moveEnemy))
        {
            packet.Write(enemyLoc);
            packet.Write(enemyRot);
            packet.Write(id);
            SendUdpDataToAll(packet);
        }
    }
    public static void InCombat(int player_id,bool isInCombat)
    {
        using (Packet packet = new Packet((int)ServerPackets.inCombat))
        {
            packet.Write(isInCombat);
            SendTcpData(player_id,packet);
        }
    }

    public static void ReturnCombatEnemies(int player_id, int[] enemies)
    {
        using (Packet packet = new Packet((int)ServerPackets.returnEnemiesInCombat))
        {
            packet.Write(enemies.Length);
            foreach (int e in enemies)
            {
                packet.Write(e);
            }
            SendTcpData(player_id, packet);
        }
    }

    public static void LoadScene(string scene_name) {

        using (Packet packet = new Packet((int)ServerPackets.load_scene)) {
            packet.Write(scene_name);
            SendTcpDataToAll(packet);
        }
    }
}
