using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance = null;                //NetworkManager instance
    public GameObject player_prefab;

    private void Awake()
    {
        GetInstance();
    }

    //singleton pattern - only one instance of NetworkManager class must exist
    private NetworkManager()
    {
        instance = this;
    }

    public static NetworkManager GetInstance()
    {
        if (instance == null)
        {
            return new NetworkManager();
        }
        return instance;
    }

    //Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);                     //keep networkmanager gameobject while loading the new scene
        QualitySettings.vSyncCount = 0;                         //unity settings about fps
        Application.targetFrameRate = 30;
        Server.StartServer(4, 25565);                           //args --> maximum players, port number
    }

    public ServerPlayer InstatiatePlayer(int client_id) {
        GameObject player;
        if (client_id == 1)
        {
            player = Instantiate(player_prefab, new Vector3(-0.94f, 1.15f, -8.9f), Quaternion.identity);     //return attached player component that has been generated
            PlayerManager.GetInstance().AddPlayer(player);
            DontDestroyOnLoad(player);
            return player.GetComponent<ServerPlayer>();
        }
        else if (client_id == 2) {
            player = Instantiate(player_prefab, new Vector3(-2.32f, 1.15f, -6.75f), Quaternion.identity);     //return attached player component that has been generated
            PlayerManager.GetInstance().AddPlayer(player);
            DontDestroyOnLoad(player);
            return player.GetComponent<ServerPlayer>();
        }
        else if (client_id == 3)
        {
            player = Instantiate(player_prefab, new Vector3(-2.29f, 1.15f, -10.63f), Quaternion.identity);     //return attached player component that has been generated
            PlayerManager.GetInstance().AddPlayer(player);
            DontDestroyOnLoad(player);
            return player.GetComponent<ServerPlayer>();
        }
        else if (client_id == 4)
        {
            player = Instantiate(player_prefab, new Vector3(-3.99f, 1.15f, -8.91f), Quaternion.identity);     //return attached player component that has been generated
            PlayerManager.GetInstance().AddPlayer(player);
            DontDestroyOnLoad(player);
            return player.GetComponent<ServerPlayer>();
        }
        Debug.Log("Player with wrong client id has been trying to instatiate in lobby...");
        return null;
    }

    private void OnApplicationQuit()                            //handle case unity doesn't properly close open connections in play mode
    {
        Server.Stop();
    }
}
