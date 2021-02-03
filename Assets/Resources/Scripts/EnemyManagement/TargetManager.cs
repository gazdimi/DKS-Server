using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager
{
    #region Singleton
    private static TargetManager instance = null;
    public static TargetManager GetInstance()
    {
        if (instance == null)
        {
            return new TargetManager();
        }
        return instance;
    }
    private TargetManager()
    {
        instance = this;
    }
    #endregion
    /// <summary>
    /// Returns the closest enemy entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="targets"></param>
    /// <returns></returns>
    public GameObject GetClosestTarget( GameObject entity,List<GameObject> targets)
    {
        float distance;
        int index = 0;
        distance = Vector3.Distance(entity.transform.position, targets[0].transform.position);
        for(int i=1; i < targets.Count; i++)
        {
            if(Vector3.Distance(entity.transform.position, targets[i].transform.position) < distance)
            {
                distance = Vector3.Distance(entity.transform.position, targets[i].transform.position);
                index = i;
            }
        }
        return targets[index];
    }
}
