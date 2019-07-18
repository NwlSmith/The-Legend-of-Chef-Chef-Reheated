using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightnessIndicator : Indicator {

    public static BrightnessIndicator instance = null;

    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        base.Awake();
    }

    protected override void UpdateValue()
    {
        Debug.Log("Brightness does not work in this version. Sorry :(");
    }
}
