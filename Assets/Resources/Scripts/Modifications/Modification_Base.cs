using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modification_Base:MonoBehaviour
{
    public GameObject mod;
    public string base_size;
    public string base_side;
    public bool used;

    public bool AttachModification(Modification modification)
    {
        if (ModificationFitsBase(modification))
        {
            mod = Rotate_Modification(modification);
            used = true;
            return true;
        }
        return false;
    }
    public void DetachModification()
    {
        mod.GetComponentInChildren<Rigidbody>().isKinematic = false;
        mod.GetComponentInChildren<Collider>().isTrigger = false;
        mod.transform.parent = null;
        mod.GetComponentInChildren<Rigidbody>().AddExplosionForce(10f, gameObject.transform.position, 3f, 0f, ForceMode.Impulse);
        mod = null;
        used = false;
    }
    public bool ModificationFitsBase(Modification modification)
    {
        if (modification.size.Equals(base_size) && !used)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Rotates the modification to be placed appropriately to fit on the enemy.
    /// </summary>
    /// <param name="mod"></param>
    /// <param name="mod_base"></param>
    /// <returns></returns>
    public GameObject Rotate_Modification(Modification mod)
    {

        GameObject modificationobj = Instantiate(Modification_Prefab_Manager.GetInstance().SearchModification(mod.type), transform.position, new Quaternion(), gameObject.transform.parent.transform);

        if (base_side == "E")
        {
            modificationobj.transform.Rotate(0, 180, 0);
        }
        else if (base_side == "N")
        {
            modificationobj.transform.Rotate(0, 0, -90);
        }
        else if (base_side == "S")
        {
            modificationobj.transform.Rotate(0, 0, 90);
        }

        return modificationobj;
    }
}
