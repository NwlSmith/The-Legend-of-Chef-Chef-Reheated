using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighscoreScreenManager : AbstractScreenManager
{
    public static HighscoreScreenManager instance = null;

    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        base.Awake();
    }
}

