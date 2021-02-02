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
        if (client_id == 0)
        {
            return Instantiate(player_prefab, new Vector3(0.75f, 0.8f, -6f), Quaternion.identity).GetComponent<Player>();     //return attached player component that has been generated
        }
        else if (client_id == 1) {
            return Instantiate(player_prefab, new Vector3(-2.8f, 0.8f, -6f), Quaternion.identity).GetComponent<Player>();     //return attached player component that has been generated
        }
        else if (client_id == 2)
        {
            return Instantiate(player_prefab, new Vector3(-2.8f, 0.8f, 6.59f), Quaternion.identity).GetComponent<Player>();     //return attached player component that has been generated
        }
        else if (client_id == 3)
        {
            return Instantiate(player_prefab, new Vector3(-2.8f, 0.8f, 0.44f), Quaternion.identity).GetComponent<Player>();     //return attached player component that has been generated
        }
        Debug.Log("Player with wrong client id has been trying to instatiate in lobby...");
        return null;
    }

    private void OnApplicationQuit()                            //handle case unity doesn't properly close open connections in play mode
    {
        Server.Stop();
    }
}
