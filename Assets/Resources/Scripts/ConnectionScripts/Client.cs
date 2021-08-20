using UnityEngine;

public class Client
{
    public int client_id;
    public TCP tcp;
    public UDP udp;
    public static int dataBufferSize = 4096;                    //bytes
    public ServerPlayer player;                                       //reference to player

    public Client(int ci)                                       //constructor of outter class Client
    {
        client_id = ci;
        tcp = new TCP(client_id);
        udp = new UDP(client_id);
    }

    public void SendToLobby(string player_name, Vector3 forward, Vector3 right)                  //send new client (local player) to lobby
    {
        player = NetworkManager.network_manager.InstatiatePlayer(client_id); //initialize player instance
        player.InitializePlayer(client_id, player_name, forward, right);
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

    public void Disconnect()                                   //disconnect client and stop traffic inside the network
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
