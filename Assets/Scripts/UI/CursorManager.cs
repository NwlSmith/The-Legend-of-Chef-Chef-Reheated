using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    public int index = 0;
    public int size;

    public Transform[] ButtonPos;
    private MenuManager menu_man;

    private AudioSource scroll_source;

    private void Awake()
    {
        scroll_source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        menu_man = GetComponentInParent<MenuManager>();

        size = ButtonPos.Length;
        index = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (index == 0)
                index = size;
            index--;
            UpdatePos();
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            index = (index + 1) % size;
            UpdatePos();
        }
    }

    private void UpdatePos()
    {
        transform.position = ButtonPos[index].position;
        menu_man.UpdateCursorPos();
        scroll_source.pitch = 1f - .05f * index;
        scroll_source.Play();
    }

    public void UpdatePos(Vector3 new_pos)
    {
        bool found = false;
        for (int i = 0; i < ButtonPos.Length; i++)
        {
            if (ButtonPos[i].position == new_pos)
            {
                index = i;
                found = true;
                break;
            }
        }
        
        if (!found)
            Debug.Log("Error: new_pos " + new_pos.ToString() + " not found");
        UpdatePos();
    }
}
