using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DungeonGenerator.GetInstance().CompleteDungeon(Stage_Manager.GetInstance().GetNextFloorRoomNumber());
    }
    public void ProceedNextStage()
    {
        StartCoroutine(Stage_Manager.GetInstance().ChangeFloor());
    }
}
