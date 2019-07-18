using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartItem : Item
{

    protected void Awake()
    {
        itemName = "Heart";
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (pm.health.GetHealth() >= 10)
                return;
            PickUp();
            pm.UpdateHealth(1);
        }
    }
}
