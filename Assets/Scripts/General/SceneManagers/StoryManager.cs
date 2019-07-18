using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour {

    public string nextSceneString;
    public Sprite[] story_images;
    protected Image story;
    protected Image black;
    protected int cur_img = 0;
    protected Coroutine fade_co;
    protected MusicManager mm;

    protected virtual void Awake()
    {
        story = transform.GetChild(0).GetComponent<Image>();
        black = transform.GetChild(1).GetComponent<Image>();
        story.sprite = story_images[0];
    }

    protected virtual void Start()
    {
        mm = MusicManager.instance;
        if (mm == null)
        {
            Debug.Log("Error: MusicManager does not exist.");
            return;
        }
        if (mm.song_source.volume == .5f)
            mm.FadeToHalfFromFull();
        if (mm.song_source.volume == 0f)
            mm.FadeInFromZero();
    }

    protected virtual void Update()
    {
        if (Input.GetButtonDown("Enter"))
            if (fade_co == null)
                fade_co = StartCoroutine(FadeToNextLevel());
        if (Input.GetButtonDown("Melee"))
        {
            cur_img++;
            if (cur_img >= story_images.Length)
                NextLevel();
            else
                story.sprite = story_images[cur_img];
        }
    }

    protected virtual void NextLevel()
    {
        if (fade_co == null)
            fade_co = StartCoroutine(FadeToNextLevel());
    }

    public IEnumerator FadeToNextLevel()
    {
        if (mm != null)
        {
            if (mm.song_source.volume == .25f)
                mm.FadeToZeroFromHalf();
            if (mm.song_source.volume == .5f)
                mm.FadeOutFromFull();
        }
        black.GetComponent<Animator>().SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        LoadNextScene();
    }

    protected void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneString, LoadSceneMode.Single);
        if (mm != null)
            mm.song_source.volume = 0f;
    }
}