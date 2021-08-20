using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionEnemyHandler 
{
    #region Singleton
    private static ConnectionEnemyHandler instance = null;
    public static ConnectionEnemyHandler GetInstance()
    {
        if (instance == null)
        {
            return new ConnectionEnemyHandler();
        }
        return instance;
    }
    ConnectionEnemyHandler()
    {
        instance = this;
        allExistingEnemies = new List<GameObject>();
    }
    #endregion
    public List<GameObject> allExistingEnemies;
}
