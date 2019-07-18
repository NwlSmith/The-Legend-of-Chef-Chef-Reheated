using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GratersItem : Item
{
    public Item spare_ladles;

    protected void Awake()
    {
        itemName = "Graters";
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Special Item: need to change player's weapon!
        if (other.tag == targetTag)
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (!pm.gm.inventory.Contains("Graters"))
            {
                GameManager.instance.RemoveItem("Ladles");
                Instantiate(spare_ladles, GetComponent<Rigidbody2D>().position, Quaternion.identity);
                pm.anim.SetBool("Ladles", false);
                pm.anim.SetBool("Graters", true);
                PickUp();
            }
        }
    }
}
