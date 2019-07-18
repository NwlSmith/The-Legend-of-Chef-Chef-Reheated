using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsScreenManager : AbstractScreenManager {

    public static ControlsScreenManager instance = null;

    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        base.Awake();
    }
}
