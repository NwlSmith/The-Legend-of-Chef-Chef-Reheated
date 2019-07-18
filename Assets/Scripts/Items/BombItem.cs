using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : Item {

    protected void Awake()
    {
        itemName = "Bomb";
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
            PickUp();
    }
}
