using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayer : MonoBehaviour                                 //[server-side] handling player's related data & logic
{
    public int player_id;
    public string username;
 
    private float[] inputs;                                          //store inputs about movement sent from client

    //joystick movement
    [SerializeField]
    public float moveSpeed = 4f;

    public Vector3 forward, right;

    public float neuro = 0;
    public float extra = 0;
    public float certainty = 0;

    public GameObject hand;
    public float activeWeapon = -1;
    List<GameObject> weapons;

    private void Start()
    {
        weapons = new List<GameObject> ();
    }

    public void InitializePlayer(int id, string usern, Vector3 forward_dir, Vector3 right_dir)              //necessary initializations
    {
        player_id = id;
        username = usern;
        forward = forward_dir;
        right = right_dir;
        inputs = new float[2];                                       //initialize the array (according to joystick movement)
    }

    public void SetInput(float[] local_inputs, Vector3 local_forward)    //inputs according to joystick keywords
    {
        inputs = local_inputs;
        transform.forward = local_forward;
    }

    public void FixedUpdate()                                       //update player's new position (movement) by making suitable calculations
    {
        if (Math.Abs(inputs[0]) > 0.2f || Math.Abs(inputs[1]) > 0.2f)//sets sensitivity for movement.
        {
            Move();
        }
    }

    private void Move()                      //change player's movement and rotation (Vector2 input_direction)
    {
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * inputs[0];
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * inputs[1];
        
        transform.position += rightMovement;
        transform.position += upMovement;

        Send.PlayerPosition(this);
        Send.PlayerRotation(this);
    }

    public void ShowWeapon(float weapon_id)
    {
        foreach (GameObject weapon in GameObject.FindGameObjectsWithTag("Equipment")) 
        {
            if(weapon.GetComponent<WeaponData>().GetID() == weapon_id)
            {
                activeWeapon = weapon_id;
                weapons.Add(weapon);
                break;
            }
        }
    }
}
