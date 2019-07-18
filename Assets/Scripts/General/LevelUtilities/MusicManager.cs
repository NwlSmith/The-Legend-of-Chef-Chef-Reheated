using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance = null;
    [HideInInspector] public AudioClip song;
    [HideInInspector] public AudioSource song_source;
    private Animator anim;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            instance.song = song;
            Destroy(gameObject);
        }

        song_source = GetComponent<AudioSource>();
        song_source.clip = song;
        anim = GetComponent<Animator>();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (song_source.playOnAwake)
            FadeInFromZero();
    }

    public void FadeInFromZero()
    {
        anim.SetTrigger("In");
        song_source.Play();
    }

    public void FadeToHalfFromFull()
    {
        if (song_source.volume != .5f)
            Debug.Log("Error: Attempting to go to half volume while not at full");
        anim.SetTrigger("ToHalf");
    }

    public void FadeToZeroFromHalf()
    {
        if (song_source.volume != .25f)
            Debug.Log("Error: Attempting to go to zero volume while not at half");
        anim.SetTrigger("ToZero");
    }

    public void FadeOutFromFull()
    {
        if (song_source.volume != .5f)
            Debug.Log("Error: Attempting to go to zero volume while not at half");
        anim.SetTrigger("Out");
        song_source.Stop();
    }
}
