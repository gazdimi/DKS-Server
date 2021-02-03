using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    [HideInInspector]
    public float range = 10f;
    [HideInInspector]
    public float speed = 10f;
    [HideInInspector]
    public float damage = 10f;
    Vector3 initpos;
    public GameObject effect;

    private void Start()
    {
        initpos = transform.position;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, initpos) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Player>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }

}
