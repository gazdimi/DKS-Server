using System.Net;

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
