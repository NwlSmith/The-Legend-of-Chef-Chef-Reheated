using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager03 : LevelManager
{
    public AudioClip doors_clip;
    private AudioSource doors_source;

    public Boss1Manager bm;

    protected override void Start()
    {
        level = 3;

        top_right_bounds = new Vector2(8, 23); //  FIGURE THESE OUT
        bottom_left_bounds = new Vector2(-8, -12);

        pm.m_light.enabled = true;

        base.Start();

        mm.song_source.Stop();
        mm.FadeOutFromFull();

        gm.StartDisplayMessage("get_to_exit");
        StartCoroutine(CheckIfChefPastDoor());

        AdjustAmbientLighting(0);

        doors_source = gameObject.AddComponent<AudioSource>();
        doors_source.clip = doors_clip;
        doors_source.playOnAwake = false;
    }

    private IEnumerator CheckIfChefPastDoor()
    {
        yield return new WaitUntil(() => pm.transform.position.y > 6);
        gm.StopDisplayMessage("get_to_exit");

        GameObject entry = GameObject.FindWithTag("EntryWall");
        entry.GetComponent<Collider2D>().enabled = true;
        entry.GetComponent<Renderer>().enabled = true;

        doors_source.Play();
        bm.HUI.gameObject.SetActive(true);
        cam.target = bm.transform;
        mm.FadeInFromZero();
        yield return new WaitForSeconds(1f);
        cam.target = pm.transform;
        yield return new WaitForSeconds(1f);
        bm.activated = true;
    }

    public override void BossDeath()
    {
        GameObject exit = GameObject.FindWithTag("ExitWall");
        exit.GetComponent<Collider2D>().enabled = false;
        exit.GetComponent<Renderer>().enabled = false;

        doors_source.Play();
        mm.FadeOutFromFull();

        base.BossDeath();
    }

    public override bool CheckVictoryCondition()
    {
        return boss_dead == true;
    }


    // handle new win conditions
    public override void LevelComplete()
    {
        StopRatMovement();
        StartCoroutine(FadeToNextLevel());
    }

}
