using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsManager : MenuManager {

    public static OptionsManager instance = null;
    public MenuManager prev_menu = null;

    private List<Indicator> indicators = new List<Indicator>();

    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        base.Awake();

        indicators.Add(transform.GetChild(2).GetComponent<Indicator>());
        indicators.Add(transform.GetChild(4).GetComponent<Indicator>());
        indicators[0].selected = true;

        DontDestroyOnLoad(gameObject);
    }

    protected override void Start()
    {
        gameObject.SetActive(false);
    }

    protected override void Update () {
        if (cm.index == 2)
            base.Update();
    }

    public override void ButtonDown()
    {
        if (cm.index == 2)
            Deactivate();
    }

    public override void UpdateCursorPos()
    {
        base.UpdateCursorPos();

        foreach (Indicator ind in indicators)
            ind.selected = false;

        switch (cm.index)
        {
            case 0:
                indicators[0].selected = true;
                break;
            case 1:
                indicators[1].selected = true;
                break;
            case 2:
                break;
            default:
                Debug.Log("Error: attempted index " + cm.index + " is invalid");
                return;
        }
    }

    public void Activate(MenuManager mm)
    {
        select_source.Play();
        (prev_menu = mm).gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        prev_menu.gameObject.SetActive(true);
        prev_menu.select_source.Play();
        gameObject.SetActive(false);
    }
}
