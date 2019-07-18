using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager02 : LevelManager
{

    protected override void Start()
    {
        level = 2;

        top_right_bounds = new Vector2(16, 6);
        bottom_left_bounds = new Vector2(-22, -16);

        pm.m_light.enabled = true;

        base.Start();
        StartCoroutine(CheckAllRatsDead());

        if (gm.inventory.Contains("Carrot"))
            gm.StartDisplayMessage("carrot_message");

        StartCoroutine(DisplayGetToExit());

        AdjustAmbientLighting(0);
    }

    private IEnumerator DisplayGetToExit()
    {
        Debug.Log("Displaying get to exit");
        if (gm.inventory.Contains("Carrot"))
        {
            Debug.Log("has carrot");

            yield return new WaitForSeconds(4f);
            gm.StartDisplayMessage("get_to_exit");
            yield return new WaitForSeconds(4);
        }
        else
        {
            gm.StartDisplayMessage("get_to_exit");
            yield return new WaitForSeconds(6f);
        }
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
        StopRatMovement();
        StartCoroutine(FadeToNextLevel());
    }
}
