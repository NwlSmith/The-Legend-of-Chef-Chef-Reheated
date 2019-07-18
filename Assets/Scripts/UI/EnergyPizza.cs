using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyPizza : MonoBehaviour {

    public static EnergyPizza instance = null;

    public Sprite[] sprites;
    [HideInInspector] Image img;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        img = GetComponent<Image>();
    }

    public void UpdateEnergy(int num)
    {
        img.sprite = sprites[num];
    }

}
