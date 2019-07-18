using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Item
{

    protected void Awake()
    {
        itemName = "Key";
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
            PickUp();
    }
}
