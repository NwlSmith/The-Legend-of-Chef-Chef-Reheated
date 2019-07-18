using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour {

    public static ItemInfo instance = null;

    public Sprite[] sprites;
    [HideInInspector] Image img;

    private Color full = new Color(1f, 1f, 1f, 1f);
    private Color transparent = new Color(1f, 1f, 1f, 0f);


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        img = GetComponent<Image>();
        img.color = transparent;
    }

    public void DisplayItem(Sprite item)
    {
        img.color = full;
        img.sprite = item;
    }

    public void TakeCurrentImageDown()
    {
        // this may make it so it cannot display another item... Not really needed.
        img.sprite = null;
        img.color = transparent;
    }
}
