using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MenuManager {


    /* 0: New Game 1: HighScore 2: Options 3: Controls 4: Quit*/

    private Image black;

    public AudioClip song;

    private Coroutine fade_co;

    protected override void Awake ()
    {
        black = transform.GetChild(7).GetComponent<Image>();
        num_choices = 5;
        base.Awake();
    }

    protected override void Start()
    {
        if (GameManager.instance != null)
            GameManager.instance = null;
        if (PauseMenuManager.instance != null)
            PauseMenuManager.instance = null;
        MusicManager.instance.song_source.clip = song;
        MusicManager.instance.FadeInFromZero();
        base.Start();
    }

    public void FadeToBlack()
    {
        black.GetComponent<Animator>().SetBool("Fade", true);
    }

    public IEnumerator FadeToNextLevel()
    {
        if (MusicManager.instance != null)
            MusicManager.instance.FadeToHalfFromFull();
        scene_transition = true;

        black.GetComponent<Animator>().SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        LoadNextScene();
    }

    protected void LoadNextScene()
    {
        SceneManager.LoadScene("Level1Intro", LoadSceneMode.Single);
    }

    public override void ButtonDown()
    {
        switch (cm.index)
        {
            case 0:
                NewGame();
                break;
            case 1:
                HighScore();
                break;
            case 2:
                Options();
                break;
            case 3:
                Controls();
                break;
            case 4:
                Quit();
                break;
            default:
                Debug.Log("Error: attempted index " + cm.index + " is invalid");
                return;
        }
    }

    public void NewGame()
    {
        if (fade_co == null)
            fade_co = StartCoroutine(FadeToNextLevel());
    }

    public void HighScore()
    {
        HighscoreScreenManager.instance.gameObject.SetActive(true);
        HighscoreScreenManager.instance.Activate(this);
    }
}
