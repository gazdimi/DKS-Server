using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Server
{
    public static int maximum_players { get; private set; }                                 //automatic property
    public static int port { get; private set; }

    private static TcpListener tcpListener = null;
    private static UdpClient udpListener = null;                                            //will manage all udp communication
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();          //(key = client's id, value = instance of Client)

    public delegate void PacketHandler(int fromClient, Packet packet);
    public static Dictionary<int, PacketHandler> packetHandlers;                            //packet's id, corresponding packet handler

    public static void StartServer(int m, int p)
    {
        maximum_players = m; port = p;

        Console.WriteLine("Starting server...\nWaiting for connections...");

        InitializedServerData();                                                                //initialize dictionary of clients (server's necessary data)
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();                                                                    //start listening for client requests
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), null);   //begin an asynchronous operation to accept an incoming connection attempt using tcp

        udpListener = new UdpClient(port);
        udpListener.BeginReceive(UdpReceivedCallback, null);                                    //start asynchronous receive using udp

        Console.WriteLine($"Server started successfully on {port}...");
    }

    private static void DoAcceptTcpClientCallback(IAsyncResult asyncResult)                     //gets called after successful client-server tcp connection and handles the newly created tcp connection  
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(asyncResult);                         //accept incoming connection and return TcpClient instance for handling remote host communication
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), null);   //continue listening for connections (once a client connects)

        Console.WriteLine($"Incoming connection from ... {client.Client.RemoteEndPoint}");
        for (int i = 1; i <= maximum_players; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(client);                                                 //connect client via tcp and initialize the connection with a handshake (send welcome packet)
                return;
            }
        }
        Console.WriteLine($"{client.Client.RemoteEndPoint} failed remote client to connect --> full server");
    }

    private static void UdpReceivedCallback(IAsyncResult asyncResult)                       //gets called after successful client-server udp attempt to connect and starts receiving incoming data via udp 
    {
        try
        {
            IPEndPoint iPEnd_client = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpListener.EndReceive(asyncResult, ref iPEnd_client);            //get incoming data via udp then end pending asynchronous receive
            udpListener.BeginReceive(UdpReceivedCallback, null);                            //continue listening for connections (once a client connects) and keep receiving data from remote client asynchronously

            if (data.Length < 4)                                                            //check if packet exists
            {
                return;
            }
            using (Packet packet = new Packet(data))
            {
                int client_id = packet.ReadInt();
                if (client_id == 0)                                                         //no existing client
                {
                    return;
                }
                if (clients[client_id].udp.iPEndPoint == null)                              //new connection (packet received from the server is an empty one, it was used only for initiating the udp connection)
                {
                    clients[client_id].udp.Connect(iPEnd_client);                           //connect new client (via udp)
                    return;                                                                 //no data to be handled
                }
                if (clients[client_id].udp.iPEndPoint.ToString() == iPEnd_client.ToString())    //check if stored endPoint matches endPoint where the packet came from
                {
                    clients[client_id].udp.HandleData(packet);
                }
            }
        }
        catch (Exception e) { 
            Console.WriteLine($"Error occurred while receiving UDP data from server: {e}"); 
        }
    }

    public static void SendUdpData(IPEndPoint iPEnd_client, Packet packet)                  //send packet from server to remote client (specified endpoint) using udp
    {
        try
        {
            if (iPEnd_client != null)
            {
                udpListener.BeginSend(packet.ToArray(), packet.Length(), iPEnd_client, null, null);     //send packet to remote client asynchronously
            }
        }
        catch (Exception e) { Console.WriteLine($"Error occurred while sending data to {iPEnd_client} via UDP: {e}"); }
    }

    private static void InitializedServerData()                                             //initialize dictionary of clients (and all necessary server data)
    {
        for (int i = 1; i <= maximum_players; i++)
        {
            clients.Add(i, new Client(i));                                                  //(key = client's id, value = instance of Client)
        }
        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int) ClientPackets.welcomeReceived, Handle.Welcome_Received},
                { (int) ClientPackets.player_movement, Handle.PlayerMovement},
                { (int) ClientPackets.shoot, Handle.Shooted },
                { (int) ClientPackets.startgame, Handle.StartGame },
                { (int) ClientPackets.pen_values, Handle.PenValues },
                { (int) ClientPackets.hold_weapon, Handle.PlayerHoldWeapon }
            };
        Console.WriteLine($"Server: initiliazation of packets have been completed");
    }

    public static void Stop() {
        tcpListener.Stop();
        udpListener.Close();
    }
}
