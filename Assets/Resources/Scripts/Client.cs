using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    public int client_id;
    public TCP tcp;
    public UDP udp;
    public static int dataBufferSize = 4096;                    //bytes
    public Player player;                                       //reference to player

    public Client(int ci)                                       //constructor of outter class Client
    {
        client_id = ci;
        tcp = new TCP(client_id);
        udp = new UDP(client_id);
    }

    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private byte[] received_buffer;
        private Packet received_packet;                         //packet (sent by client) and received from server

        public TCP(int i)                                       //constructor of inner class TCP
        {
            id = i;
        }

        public void Connect(TcpClient client_socket)            //initialize necessary instances about tcp connection with the client of newly connected client (specified TcpClient)
        {
            socket = client_socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();                        //get the NetworkStream used to send and receive data from TcpClient
            received_packet = new Packet();                     //initialize packet instance
            received_buffer = new byte[dataBufferSize];
            stream.BeginRead(received_buffer, 0, dataBufferSize, TcpReceiveCallback, null); //begin to read from NetworkStream asynchronously

            Send.Welcome(id, "Welcome to the server!!");        //once client-server communication has been established, send a welcome packet from server to the client (handshake)
        }

        private void TcpReceiveCallback(IAsyncResult asyncResult) //read incoming packet-data from NetworkStream
        {
            try
            {
                int byte_length = stream.EndRead(asyncResult);  //return number of bytes that have been read from NetworkStream then end the asynchronous read
                if (byte_length <= 0)
                {
                    Server.clients[id].Disconnect();                //will disconnect both tcp and udp connections
                    return;
                }
                byte[] data = new byte[byte_length];                //if data has been received, create new buffer for the data
                Array.Copy(received_buffer, data, byte_length);     //copy from one array to another
                received_packet.Reset(HandleData(data));            //reset Packet instance so it can be reused, but first get data from the packet
                stream.BeginRead(received_buffer, 0, dataBufferSize, TcpReceiveCallback, null);    //continue reading data from the NetworkStream
            }
            catch (Exception e)
            {
                Debug.Log($"Error occured while receiving TCP data: {e}");
                Server.clients[id].Disconnect();                    //will disconnect both tcp and udp connections
            }
        }

        public void SendData(Packet packet)                     //send data-packet to the client using tcp
        {
            try
            {
                if (socket != null)                             //check if client's socket has been initialized
                {

                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error sending packet data to player (client) with {id} via TCP {e}...");
            }
        }

        private bool HandleData(byte[] data)                        //returns a boolean, telling Packet's Reset() whether the instance should be cleared
        {                                                           //prepare received data-packet to get used by suitable packet handler method
            int packet_length = 0;
            received_packet.SetBytes(data);                         //set packet's bytes to the ones we read from the stream (data)
            if (received_packet.UnreadLength() >= 4)                //this is the start of packet (first value placed in the packet is its content's length, which is an integer [int consists of 4 bytes])
            {
                packet_length = received_packet.ReadInt();          //get packet's length of data that was sent from client and received from server
                if (packet_length <= 0)                             //no data stored inside the packet
                {
                    return true;                                    //true --> reset packet in order to receive new data
                }
            }

            while (packet_length > 0 && packet_length <= received_packet.UnreadLength())     //received packet contains another complete (whole) packet that needs to handled
            {                                                                               //and as long as packet's length doesn't exceed the length of the one we are reading
                byte[] packet_bytes = received_packet.ReadBytes(packet_length);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packet_bytes))
                    {
                        int packet_id = packet.ReadInt();
                        Server.packetHandlers[packet_id](id, packet);              //invoke passing packet instance (call appropriate method to handle Packet)
                    }
                });

                packet_length = 0;                                      //reset packet's length
                if (received_packet.UnreadLength() >= 4)                //this is the start of packet (first value placed in the packet is its content's length, which is an integer [int consists of 4 bytes])
                {
                    packet_length = received_packet.ReadInt();          //get packet's length of data that was sent from client and received from server
                    if (packet_length <= 0)                             //no data stored inside
                    {
                        return true;                                    //true --> reset packet in order to receive new data
                    }
                }
            }

            if (packet_length <= 1)
            {
                return true;
            }
            return false;                                               //partial packet exists, so don't reset }
        }

        public void Disconnect()                                    //close tcp connections and clean necessary TCP instances
        {
            socket.Close();                                         //dispose TcpClient instance
            stream = null;                                          //empty NetworkStream
            received_packet = null;                                 //empty Packet instance
            received_buffer = null;
            socket = null;
        }
    }

    public class UDP
    {

        public IPEndPoint iPEndPoint;                           //represents a network endpoint as an IP address and a port number
        private int client_id;                                  //will store client's id of udp connection

        public UDP(int id)                                      //constructor of inner class UDP
        {
            client_id = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            iPEndPoint = endPoint;
        }

        public void SendData(Packet packet)                     //send packet from server to client via udp
        {
            Server.SendUdpData(iPEndPoint, packet);
        }

        public void HandleData(Packet packet_data)              //prepare received data-packet to get used by suitable packet handler method
        {
            int packet_length = packet_data.ReadInt();
            byte[] packet_bytes = packet_data.ReadBytes(packet_length);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packet_bytes))            //new Packet with the given data
                {
                    int packet_id = packet.ReadInt();                       //extract packet's content
                    Server.packetHandlers[packet_id](client_id, packet);    //invoke passing packet instance ((call appropriate method to handle Packet)
                }
            });
        }

        public void Disconnect()
        {
            iPEndPoint = null;
        }
    }

    public void SendToGame(string player_name)                  //send new client (local player) to game field and inform other remote clients about his existence
    {
        player = NetworkManager.network_manager.InstatiatePlayer(); //initialize player instance
        player.InitializePlayer(client_id, player_name);
        foreach (Client client in Server.clients.Values)
        {                                                       //send info from all other players (already connected to the new connected player)
            if (client.player != null)
            {
                if (client.client_id != client_id)                          //for every remote client except local player
                {
                    Send.Generate(client_id, client.player);
                }
            }
        }
        foreach (Client client in Server.clients.Values)                    //send new player's info to all other remote clients (players)
        {
            if (client.player != null)
            {
                Send.Generate(client.client_id, player);
            }
        }
    }

    private void Disconnect()                                   //disconnect client and stop traffic inside the network
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} with username {player.username} has disconnected from the game...");
        ThreadManager.ExecuteOnMainThread(() => {
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;                                      //destroy Player object from main thread
        });
        tcp.Disconnect();
        udp.Disconnect();
        Send.DisconnectPlayer(client_id);                       //when client (local player's disconnects, inform all other connected players) 
    }
}
