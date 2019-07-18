using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractScreenManager : MonoBehaviour
{
    public MenuManager prev_menu = null;
    protected AudioSource select_source;

    protected virtual void Awake()
    {
        select_source = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Enter"))
        {
            Deactivate();
        }
    }

    public void Activate(MenuManager mm)
    {
        (prev_menu = mm).gameObject.SetActive(false);
        select_source.Play();
    }

    public void Deactivate()
    {
        prev_menu.gameObject.SetActive(true);
        prev_menu.select_source.Play();
        gameObject.SetActive(false);
    }
}

