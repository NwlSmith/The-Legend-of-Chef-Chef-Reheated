using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager04 : LevelManager
{

    protected override void Start()
    {
        level = 4;

        top_right_bounds = new Vector2(31, 17);
        bottom_left_bounds = new Vector2(-23, -13);

        pm.m_light.enabled = true;

        base.Start();

        AdjustAmbientLighting(.01f);
    }

    public override bool CheckVictoryCondition()
    {
        //must close all of the pipes
        return pipes_closed;
    }

    // handle new win conditions
    public override void LevelComplete()
    {
        mm.FadeOutFromFull();
        StopRatMovement();
        StartCoroutine(FadeToNextLevel());
    }
}
