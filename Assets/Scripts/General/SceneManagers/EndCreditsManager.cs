using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCreditsManager : StoryManager {

    public AudioClip end_song;

    protected override void Start()
    {
        mm = MusicManager.instance;
        if (mm == null)
        {
            Debug.Log("Error: MusicManager does not exist.");
            return;
        }

        mm.song_source.clip = end_song;
        mm.song_source.volume = 0f;
        mm.FadeInFromZero();
        mm.song_source.Play();
    }

    protected override void NextLevel()
    {
        mm.FadeOutFromFull();
        base.NextLevel();
    }
}
