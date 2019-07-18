using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour {

    public string itemName;
    public string group;
    public int number;

    public void AddOne()
    {
        if (itemName != "Bomb" && itemName != "Carrot")
            Debug.Log("Attempting to add another of item " + itemName + ", which is not allowed.");
        else
        {
            number++;
            GetComponentInChildren<Text>().text = number.ToString();
        }
    }

    public void RemoveOne()
    {
        if (itemName != "Bomb" && itemName != "Carrot")
            Debug.Log("Attempting to Remove one of item " + itemName + ", which is not allowed.");
        else
        {
            number--;
            GetComponentInChildren<Text>().text = number.ToString();
        }
    }
}
