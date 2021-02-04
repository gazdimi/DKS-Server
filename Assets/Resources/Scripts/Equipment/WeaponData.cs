using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public float id;
    public float GetID()
    {
        return id;
    }

    public void SetID(float id)
    {
        this.id = id;
    }
}
