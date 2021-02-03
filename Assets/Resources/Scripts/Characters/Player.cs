using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour,Character
{
    [SerializeField]
    [Range(1, 500)]
    protected float hp = 10;
    private bool combatant = false;
    [HideInInspector]
    public bool overEquipment = false;

    protected List<GameObject> weapons;

    //current weapon on hand
    protected int currentWeaponIndex = 0;

    //object currently interacting with.
    [HideInInspector]
    public GameObject interactObject;

    public GameObject hand;
    public GameObject body;

    //HealthBar hpbar;

    private void Start()
    {
        weapons = new List<GameObject> { null, null };
       // hpbar = transform.parent.Find("HealthCanvas").GetComponentInChildren<HealthBar>();
       // hpbar.InitHealth(hp);
    }

    public void enterCombat()
    {
        this.combatant = true;
    }
    public void exitCombat()
    {
        this.combatant = false;
        body.transform.localRotation = Quaternion.identity;
    }

    public bool InCombat()
    {
        return combatant;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
       // hpbar.SetHealth(hp);
    }

    public void Kill(float delay)
    {
        Destroy(gameObject, delay);
    }

    public GameObject Closest(List<GameObject> targets)
    {
        try
        {
            float distance = Vector3.Distance(gameObject.transform.position, targets[0].transform.position);
            GameObject closest = targets[0];
            foreach (GameObject t in targets)
            {
                float d = Vector3.Distance(gameObject.transform.position, t.transform.position);
                if (d < distance)
                {
                    distance = d;
                    closest = t;
                }
            }
            return closest;
        }
        catch
        {
            return null;
        }
        
    }

    public void InteractClicked()
    {
        if (overEquipment)
        {
            overEquipment = false;
          //  interactObject.gameObject.GetComponent<EquipmentOnGround>().PickedUp(true);
            if (weapons[currentWeaponIndex] != null)
            {
                if (weapons[NextItem()] != null)
                {
                    DropWeapon();
                }
                else
                {
                    ChangeWeapon();
                }
            }
            TakeWeapon();
        }
        else
        {
            if (weapons[NextItem()]!=null)
            {
                ChangeWeapon();
            }
        }
    }

    int NextItem()
    {
        int pos;
        if (currentWeaponIndex < weapons.Count - 1)
        {
            pos = currentWeaponIndex+1;
        }
        else
        {
            pos = 0;
        }
        return pos;
    }
    
    void ShowWeapon(GameObject shown)
    {
        shown.SetActive(true);
        shown.transform.worldToLocalMatrix.MultiplyVector(hand.transform.forward);
        shown.transform.parent = hand.transform;
        shown.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    void TakeWeapon()
    {
        //create the weapon on player's hand
        weapons[currentWeaponIndex] =  interactObject;
        interactObject.transform.position = hand.transform.position;
        ShowWeapon(weapons[currentWeaponIndex]);
    }

    void ChangeWeapon()
    {
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = NextItem();
        if (weapons[currentWeaponIndex]!=null)
        {
            weapons[currentWeaponIndex].SetActive(true);
        }
    }

    void DropWeapon()
    {
        weapons[currentWeaponIndex].transform.parent = null;
      //  weapons[currentWeaponIndex].GetComponent<EquipmentOnGround>().PickedUp(false);
        weapons[currentWeaponIndex] = null;
    }

    public void Attack()
    {
        bool attacked = false;
        if (weapons[currentWeaponIndex] != null)
        {
      //      attacked = weapons[currentWeaponIndex].GetComponent<Weapon>().Attack(GetComponent<LockOnTarget>().targeted);
        }
        //depending on the result, alter neuroticism.
      //  GetComponent<CharacterBehaviour>().pen.AttackNeuro(attacked);
    }
}
