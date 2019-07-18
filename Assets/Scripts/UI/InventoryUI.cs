using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

    public static InventoryUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void Add(string item)
    {
        if (item == "Ladles")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            return;
        }
        else if (item == "Graters")
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            return;
        }
        else if (item == "Bomb")
        {
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.GetComponent<InventoryItem>().AddOne();
            return;
        }
        else if (item == "KnifeSet")
        {
            transform.GetChild(3).gameObject.SetActive(true);
            return;
        }
        else if (item == "Carrot")
        {
            transform.GetChild(4).gameObject.SetActive(true);
            transform.GetChild(4).gameObject.GetComponent<InventoryItem>().AddOne();
            return;
        }
        else if (item == "Key")
        {
            transform.GetChild(5).gameObject.SetActive(true);
            return;
        }


        else
        {
            Debug.Log("Invalid Add of item " + item);
        }

    }

    public void Remove(string item)
    {
        if (item == "Ladles")
        {
            transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        else if (item == "Graters")
        {
            transform.GetChild(1).gameObject.SetActive(false);
            return;
        }
        else if (item == "Bomb")
        {
            InventoryItem invItem = transform.GetChild(2).gameObject.GetComponent<InventoryItem>();
            invItem.RemoveOne();
            if (invItem.number <= 0)
            {
                transform.GetChild(2).gameObject.SetActive(false);
            }
            return;
        }
        else if (item == "KnifeSet")
        {
            transform.GetChild(3).gameObject.SetActive(false);
            return;
        }
        else if (item == "Carrot")
        {
            InventoryItem invItem = transform.GetChild(4).gameObject.GetComponent<InventoryItem>();
            invItem.RemoveOne();
            if (invItem.number <= 0)
            {
                transform.GetChild(4).gameObject.SetActive(false);
            }
            return;
        }
        else if (item == "Key")
        {
            transform.GetChild(5).gameObject.SetActive(false);
            return;
        }


        else
        {
            Debug.Log("Invalid Remove of item " + item);
        }
    }
}
