using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager05 : LevelManager
{

    protected override void Start()
    {
        level = 5;

        top_right_bounds = new Vector2(19, 28);
        bottom_left_bounds = new Vector2(-12, -28);

        pm.m_light.enabled = true;

        base.Start();

        StartCoroutine(DisplayGetToExit());

        AdjustAmbientLighting(.01f);
    }

    private IEnumerator DisplayGetToExit()
    {
        gm.StartDisplayMessage("get_to_exit");
        yield return new WaitForSeconds(6f);
        gm.StopDisplayMessage("get_to_exit");
    }

    public override bool CheckVictoryCondition()
    {
        // only requirement is that you get to the exit
        return true;
    }

    // handle new win conditions
    public override void LevelComplete()
    {
        mm.FadeOutFromFull();
        StopRatMovement();
        StartCoroutine(FadeToNextLevel());
    }
}
