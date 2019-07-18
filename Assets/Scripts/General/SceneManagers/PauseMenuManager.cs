using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MenuManager
{

    public bool paused = false;

    /* 0: Resume 1: Options 2: Controls 3: Quit*/

    public GameObject[] children;


    public static PauseMenuManager instance = null;

    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        base.Awake();
    }

    protected override void Start()
    {
        foreach (GameObject go in children)
            go.SetActive(false);
    }

    protected override void Update()
    {
        if (paused)
            base.Update();
    }

    public override void ButtonDown()
    {
        switch (cm.index)
        {
            case 0:
                Resume();
                break;
            case 1:
                Options();
                break;
            case 2:
                Controls();
                break;
            case 3:
                Quit();
                break;
            default:
                Debug.Log("Error: attempted index " + cm.index + " is invalid");
                return;
        }
    }

    public void Pause()
    {
        select_source.Play();
        TogglePaused();
        if (GameManager.instance.debug)
            Debug.Log("Pause");
        foreach (GameObject go in children)
            go.SetActive(true);
    }

    public void Resume()
    {
        if (GameManager.instance.debug)
            Debug.Log("Resume");
        foreach (GameObject go in children)
            go.SetActive(false);
        TogglePaused();
        GameManager.instance.UnpauseGame();
    }

    public void TogglePaused()
    {
        paused = !paused;
    }
}
