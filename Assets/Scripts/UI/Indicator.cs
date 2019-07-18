using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Indicator : MonoBehaviour {

    public int cur_index = 5;
    [HideInInspector] public bool selected = false;
    protected List<GameObject> units = new List<GameObject>();
    protected AudioSource scroll_source;

    protected virtual void Awake()
    {
        foreach (Transform child in transform)
            units.Add(child.gameObject);

        UpdateIndicator();
        scroll_source = GetComponent<AudioSource>();
        
    }

    private void Update()
    {
        if (selected)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                cur_index--;
                if (cur_index < 0)
                    cur_index = 0;
                else
                    UpdateIndicator();
            }

            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                cur_index++;
                if (cur_index >= units.Count)
                    cur_index = units.Count - 1;
                else
                    UpdateIndicator();
            }
        }
        
    }

    private void UpdateIndicator()
    {
        if (scroll_source != null)
            scroll_source.Play();
        for (int i = 0; i < units.Count; i++)
        {
            GameObject go = units[i];
            go.SetActive(false);
            if (i <= cur_index)
                go.SetActive(true);
        }
        UpdateValue();
    }

    protected abstract void UpdateValue();


}
