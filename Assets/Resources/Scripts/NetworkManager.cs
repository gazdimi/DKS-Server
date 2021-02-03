using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager network_manager;                //NetworkManager instance
    public GameObject player_prefab;

    private void Awake()
    {
        if (network_manager == null)
        {
            network_manager = this;                             //set it equal to the instance of NetworkManager class
        }
        else if (network_manager != this)
        {
            Debug.Log("Incorrect instance needs to be destroyed...");
            Destroy(this);                                      //only one instance of NetworkManager class must exist
        }
    }

    //Start is called before the first frame update
    private void Start()
    {
        QualitySettings.vSyncCount = 0;                         //unity settings about fps
        Application.targetFrameRate = 30;
        Server.StartServer(4, 26950);
    }

    public Player InstatiatePlayer(int client_id) {
        GameObject player;
        if (client_id == 1)
        {
            player = Instantiate(player_prefab, new Vector3(-0.94f, 1.15f, -8.9f), Quaternion.identity);     //return attached player component that has been generated
            PlayerManager.GetInstance().AddPlayer(player);
            DontDestroyOnLoad(player);
            return player.GetComponent<Player>();
        }
        else if (client_id == 2) {
            player = Instantiate(player_prefab, new Vector3(-2.32f, 1.15f, -6.75f), Quaternion.identity);     //return attached player component that has been generated
            PlayerManager.GetInstance().AddPlayer(player);
            DontDestroyOnLoad(player);
            return player.GetComponent<Player>();
        }
        else if (client_id == 3)
        {
            player = Instantiate(player_prefab, new Vector3(-2.29f, 1.15f, -10.63f), Quaternion.identity);     //return attached player component that has been generated
            PlayerManager.GetInstance().AddPlayer(player);
            DontDestroyOnLoad(player);
            return player.GetComponent<Player>();
        }
        else if (client_id == 4)
        {
            player = Instantiate(player_prefab, new Vector3(-3.99f, 1.15f, -8.91f), Quaternion.identity);     //return attached player component that has been generated
            PlayerManager.GetInstance().AddPlayer(player);
            DontDestroyOnLoad(player);
            return player.GetComponent<Player>();
        }
        Debug.Log("Player with wrong client id has been trying to instatiate in lobby...");
        return null;
    }

    private void OnApplicationQuit()                            //handle case unity doesn't properly close open connections in play mode
    {
        Server.Stop();
    }
}
