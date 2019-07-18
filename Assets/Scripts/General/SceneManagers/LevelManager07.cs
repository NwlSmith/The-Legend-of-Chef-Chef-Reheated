using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager07 : LevelManager
{
    public Rigidbody2D[] spawns;
    public AudioClip doors_clip;

    public Boss2Manager bm;

    protected override void Start()
    {
        level = 7;

        cam.top_right_bounds = top_right_bounds = new Vector2(22, 20);
        cam.bottom_left_bounds = bottom_left_bounds = new Vector2(-22, -13);

        pm.m_light.enabled = true;

        gm = GameManager.instance;
        if (gm == null)
            Debug.Log("Error: GameManager does not exist.");
        mm = MusicManager.instance;
        if (mm == null)
            Debug.Log("Error: MusicManager does not exist.");

        gm.cur_level = level;

        mm.song_source.clip = level_song;
        mm.FadeInFromZero();

        GameObject[] sp = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject go in sp)
            spawnPoints.Add(go.transform);

        CompileListOfLights();
        SetLevelEntityLighting();

        AdjustAmbientLighting(0);
    }

    public override void SpawnObjects()
    {
        foreach (Transform trans in spawnPoints)
        {
            int num = Random.Range(0, spawns.Length);
            if (num < 4)
                Instantiate(spawns[num], trans.position, Quaternion.identity);
            else
                SpawnRat(trans.position);
        }
    }

    // have boss music fade away and win music fade in? or maybe gamemanager should deal w that? OH I could just make it so the storyscene is added on top?
    public override void BossDeath()
    {
        LevelComplete();
    }

    public override bool CheckVictoryCondition()
    {
        return boss_dead == true;
    }


    // handle new win conditions
    public override void LevelComplete()
    {
        mm.FadeOutFromFull();
        StopRatMovement();
        StartCoroutine(FadeToNextLevel());
    }

}
