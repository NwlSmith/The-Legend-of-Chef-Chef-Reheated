using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager01 : LevelManager {
    
    protected override void Start()
    {
        level = 1;

        top_right_bounds = new Vector2(21, 7);
        bottom_left_bounds = new Vector2(-15, -15);

        base.Start();

        StartCoroutine(CheckAllRatsDead());

        //if level is level 1
        gm.StartDisplayMessage("rats_escaped");
        gm.StartDisplayMessage("need_weapon");

        AdjustAmbientLighting(1);

        //ENABLE THIS IN NEXT LEVEL
        pm.m_light.enabled = false;
    }

    public override bool CheckVictoryCondition()
    {
        return rats_dead && found_key;
    }

    public override void LevelComplete()
    {
        gm.RemoveItem("Key");
        StopRatMovement();
        StartCoroutine(FadeToNextLevel());
    }
}
