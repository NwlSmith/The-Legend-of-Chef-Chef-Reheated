using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaItem : Item
{

    protected void Awake()
    {
        itemName = "Pizza";
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (pm.chef_energy >= 8)
                return;
            PickUp();
            pm.UpdateEnergy(1);
        }
    }
}
