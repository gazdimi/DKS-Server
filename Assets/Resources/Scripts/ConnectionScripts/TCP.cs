using System;
using System.Net.Sockets;
using UnityEngine;

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
        socket.ReceiveBufferSize = Client.dataBufferSize;
        socket.SendBufferSize = Client.dataBufferSize;

        stream = socket.GetStream();                        //get the NetworkStream used to send and receive data from TcpClient
        received_packet = new Packet();                     //initialize packet instance
        received_buffer = new byte[Client.dataBufferSize];
        stream.BeginRead(received_buffer, 0, Client.dataBufferSize, TcpReceiveCallback, null); //begin to read from NetworkStream asynchronously

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
            stream.BeginRead(received_buffer, 0, Client.dataBufferSize, TcpReceiveCallback, null);    //continue reading data from the NetworkStream
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
