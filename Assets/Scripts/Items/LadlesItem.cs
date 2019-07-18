using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadlesItem : Item
{

    protected void Awake()
    {
        itemName = "Ladles";
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (!pm.gm.inventory.Contains("Graters") && !pm.gm.inventory.Contains("Ladles"))
            {
                pm.gm.StopDisplayMessage("need_weapon");
                pm.anim.SetBool("Ladles", true);
                PickUp();
            }
        }
    }
}
