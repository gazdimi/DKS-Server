using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Possibility
{
    private string item;
    private float value;

    public Possibility(string Item,float Value)
    {
        item = Item;
        value = Value;
    }
    public string GetItem() {
        return item;
    }
    public float GetValue()
    {
        return value;
    }
    public void SetValue(float newval)
    {
        value = newval;
    }
}
