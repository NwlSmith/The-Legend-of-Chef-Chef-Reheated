using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotItem : Item
{

    protected void Awake()
    {
        itemName = "Carrot";
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
        {
            GameManager gm = GameManager.instance;
            if (gm == null)
            {
                Debug.Log("Error: GameManager does not exist.");
                return;
            }
            else if (gm.cur_level > 1 && !gm.inventory.Contains("Carrot"))
                gm.StartDisplayMessage("carrot_message");
            PickUp();
        }
    }
}
