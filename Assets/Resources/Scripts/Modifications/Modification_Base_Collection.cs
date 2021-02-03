using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modification_Base_Collection
{
    private List<Modification_Base> collection;
    public Modification_Base_Collection()
    {
        collection = new List<Modification_Base>();
    }

    public void AddModificationBase(Modification_Base mBase)
    {
        collection.Add(mBase);
    }

    public List<GameObject> GetAllModifications()
    {
        List<GameObject> mods = new List<GameObject>();
        foreach(Modification_Base mBase in collection)
        {
            if (mBase.used)
            {
                mods.Add(mBase.mod);
            }
        }
        return mods;
    }
    public List<Modification_Base> GetAllBases()
    {
        return collection;
    }
}
