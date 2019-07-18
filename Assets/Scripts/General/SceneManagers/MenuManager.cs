using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuManager : MonoBehaviour {

    protected int num_choices;
    
    public AudioSource select_source;
    protected bool scene_transition = false;

    [HideInInspector] public CursorManager cm;
    
    protected virtual void Awake()
    {
        select_source = GetComponent<AudioSource>();
        cm = GetComponentInChildren<CursorManager>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (Input.GetButtonDown("Enter") && !scene_transition)
        {
            select_source.Play();
            ButtonDown();
        }
    }

    public abstract void ButtonDown();

    public virtual void UpdateCursorPos()
    {
    }

    public void Options()
    {
        OptionsManager.instance.gameObject.SetActive(true);
        OptionsManager.instance.Activate(this);
    }

    public void Controls()
    {
        ControlsScreenManager.instance.gameObject.SetActive(true);
        ControlsScreenManager.instance.Activate(this);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
