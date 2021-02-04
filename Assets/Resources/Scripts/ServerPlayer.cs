using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayer : MonoBehaviour                                 //[server-side] handling player's related data & logic
{
    public int player_id;
    public string username;
 
    public float gravity = (-9.81f) * 2;                                  //gravity acceleration

    public float moving_speed = 5f;                                 //player's move speed (calculate like multiplying by unity's time.deltatime)
    public float jumping_speed = 5f;
    private float vertical_speed_y = 0;
    public Transform shoot_point;
    public float current_health;
    public float maximum_health = 100f;
    private float[] inputs;                                          //store inputs about movement sent from client

    //joystick movement
    [SerializeField]
    public float moveSpeed = 4f;

    public Vector3 forward, right;

    public CharacterController control;

    public float neuro = 0;
    public float extra = 0;
    public float certainty = 0;

    public GameObject hand;
    public float activeWeapon = -1;
    List<GameObject> weapons;

    private void Start()
    {
        gravity *= Mathf.Pow(Time.fixedDeltaTime, 2);
        moving_speed *= Time.fixedDeltaTime;
        jumping_speed *= Time.fixedDeltaTime;
        weapons = new List<GameObject> ();
    }

    public void InitializePlayer(int id, string usern, Vector3 forward_dir, Vector3 right_dir)              //necessary initializations
    {
        player_id = id;
        username = usern;
        current_health = maximum_health;
        forward = forward_dir;
        right = right_dir;
        inputs = new float[2];                                       //initialize the array (according to joystick movement)
    }

    public void SetInput(float[] local_inputs)//, Quaternion local_rotation)    //inputs according to pressed keywords, new rotation according to mouse input
    {
        inputs = local_inputs;
        //transform.rotation = local_rotation;
    }

    public void FixedUpdate()                                       //update player's new position (movement) by making suitable calculations
    {
        if (current_health <= 0f)                                   //dead player can't move anymore 
        {
            return;
        }
        if (Math.Abs(inputs[0]) > 0.2f || Math.Abs(inputs[1]) > 0.2f)//sets sensitivity for movement.
        {
            Move();
        }
    }

    private void Move()                      //change player's movement and rotation (Vector2 input_direction)
    {
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * inputs[0];
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * inputs[1];

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading; //in order to move according to camera rotation and NOT global position.
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

    public void Shoot(Vector3 facing_direction) 
    {
        if (Physics.Raycast(shoot_point.position, facing_direction, out RaycastHit raycastHit, 20f)) //check if shooting ray intersects with a Collider (hit of length 20f)
        {
            if (raycastHit.collider.CompareTag("Player"))                                           //check if hitted collider was tagged as Player
            {
                raycastHit.collider.GetComponent<ServerPlayer>().ReceiveDamage(10f);
            }
        }
    }

    public void ReceiveDamage(float damage) 
    {
        if (current_health <= 0f)                                                                    //no life
        {
            return;
        }
        current_health -= damage;                                                                   //manage health
        if (current_health <= 0f) 
        {
            current_health = 0f;                                                                    //negative health points don't exist
            //characterController.enabled = false;                                                    //start again from the beginning of the level
            transform.position = new Vector3(0f, 2f, 0f);                                           //[e.x. as a start position]
            Send.PlayerPosition(this);                                                              //inform all other connected players (so that the shooted player gets moved in all remote client's game field instances)
            StartCoroutine(Regenerate());
        }
        Send.PlayerHealth(this);
    }

    private IEnumerator Regenerate()                                //regenerate player after his death
    {
        yield return new WaitForSeconds(6f);                        //[add delay] wait for some seconds then regenerate player
        current_health = maximum_health;
        //characterController.enabled = true;
        Send.RegeneratePlayer(this);
    }
}
