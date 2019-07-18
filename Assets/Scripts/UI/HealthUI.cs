using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour {

    public int maxHealth;

    public void UpdateUI(int health)
    {
        for (int i = 0; i < maxHealth; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < health; i++)
            transform.GetChild(i).gameObject.SetActive(true);
    }

}
