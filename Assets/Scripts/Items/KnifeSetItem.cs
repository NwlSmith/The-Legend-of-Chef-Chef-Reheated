using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeSetItem : Item
{

    protected void Awake()
    {
        itemName = "KnifeSet";
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
            if (!GameManager.instance.inventory.Contains("KnifeSet"))
                PickUp();
    }
}
