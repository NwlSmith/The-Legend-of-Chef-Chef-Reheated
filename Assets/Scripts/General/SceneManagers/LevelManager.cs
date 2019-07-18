using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class LevelManager : MonoBehaviour {

    public static LevelManager instance = null;

    public int level;
    public Exit exit;
    public bool rats_dead = false;
    public bool boss_dead = false;
    public bool found_key = false;
    public bool pipes_closed = false;
    public int num_pipes_open;

    public EnemyManager[] enemyPrefabs;
    [HideInInspector] public List<Transform> spawnPoints = new List<Transform>();
    public Transform[] CamPath;

    public Vector3 top_right_bounds;
    public Vector3 bottom_left_bounds;

    public List<EnemyManager> enemiesInScene = new List<EnemyManager>();
    public List<Light> lights_in_scene = new List<Light>();

    public Light dir_light;
    public CameraFollow cam;
    public Image black;

    public PlayerManager pm;
    [HideInInspector] public GameManager gm;

    public string nextSceneString;

    public AudioClip level_song;
    protected MusicManager mm;

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    protected virtual void Start () {
        gm = GameManager.instance;
        if (gm == null)
            Debug.Log("Error: GameManager does not exist.");
        mm = MusicManager.instance;
        if (mm == null)
            Debug.Log("Error: MusicManager does not exist.");

        mm.song_source.Stop();
        mm.song_source.clip = level_song;
        mm.FadeInFromZero();

        gm.cur_level = level;

        cam.top_right_bounds = top_right_bounds;
        cam.bottom_left_bounds = bottom_left_bounds;

        GameObject[] sp = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject go in sp)
        {
            spawnPoints.Add(go.transform);
            SpawnRat(go.transform.position);
        }

        CompileListOfLights();
        SetLevelEntityLighting();

        num_pipes_open = GameObject.FindGameObjectsWithTag("EnemySpawner").Length;
    }



    private void Update()  // Cheats!
    {
        if (gm.debug)
        {
            if (Input.GetKeyDown(KeyCode.K))
                KillAllRats();
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                nextSceneString = "Level1Intro";
                LevelComplete();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                nextSceneString = "Level2Intro";
                LevelComplete();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                nextSceneString = "Level3Intro";
                LevelComplete();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                nextSceneString = "Level4Intro";
                LevelComplete();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                nextSceneString = "Level5";
                LevelComplete();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                nextSceneString = "Level6";
                LevelComplete();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                nextSceneString = "Level7Intro";
                LevelComplete();
            }
            else if (Input.GetKeyDown(KeyCode.G))
                gm.AddItem("Bomb");
            else if (Input.GetKeyDown(KeyCode.H))
                gm.playerManager.UpdateHealth(1);
            else if (Input.GetKeyDown(KeyCode.E))
                if (gm.playerManager.chef_energy < 8)
                    gm.playerManager.UpdateEnergy(1);
        }
    }


    public EnemyManager SpawnRat(Vector3 pos)
    {
        EnemyManager enemyInstance = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], pos, Quaternion.identity) as EnemyManager;
        enemyInstance.GetComponent<Pathfinding.AIDestinationSetter>().target = pm.transform;
        enemiesInScene.Add(enemyInstance);
        return enemyInstance;
    }

    public virtual void SpawnObjects()
    {
    }
    
    public void SendRatsToSpawn()
    {
        for (int i = 0; i < enemiesInScene.Count; i++)
        {
            enemiesInScene[i].GoToSpawn();
        }
    }

    public void StopRatMovement()
    {
        for (int i = 0; i < enemiesInScene.Count; i++)
        {
            enemiesInScene[i].StopMoving();
        }
    }

    public void KillAllRats()
    {
        for (int i = 0; i < enemiesInScene.Count; i++)
        {
            enemiesInScene[i].UpdateHealth(-100);
        }
    }

    /* ------ Lighting ------ */

    protected void CompileListOfLights()
    {
        lights_in_scene.Add(pm.GetComponentInChildren<Light>());
        for (int i = 0; i < enemiesInScene.Count; i++)
        {
            lights_in_scene.Add(enemiesInScene[i].GetComponentInChildren<Light>());
        }
        PantryContainer[] pc_list = FindObjectsOfType<PantryContainer>();
        for (int i = 0; i < pc_list.Length; i++)
        {
            lights_in_scene.Add(pc_list[i].GetComponentInChildren<Light>());
        }
    }

    protected void SetLevelEntityLighting()
    {
        for (int i = 0; i < gm.inventory.Count; i++)
            if (gm.inventory[i] == "Carrot")
                AdjustLights();
    }

    public void RemoveLight(Light l)
    {
        lights_in_scene.Remove(l);
    }

    public void AdjustLights()
    {
        // we don't want there to be lighting in first scene.
        if (level == 1)
            return;
        for (int i = 0; i < lights_in_scene.Count; i++)
            if (lights_in_scene[i] != null)
                StartCoroutine(IncreaseLightRange(lights_in_scene[i]));
    }

    protected void AdjustAmbientLighting(float new_val)
    {
        dir_light.intensity = new_val;
    }

    protected IEnumerator IncreaseLightRange(Light l)
    {
        WaitForSeconds inc_wait = new WaitForSeconds(.05f);
        float target_range = l.range + 5;
        while (l != null && l.range < target_range)
        {
            l.range = Mathf.Lerp(l.range, target_range, .15f);
            yield return inc_wait;
        }
    }

    /* ------ Camera Movements ------ */

    public void CameraShake(float intensity, float duration, bool fade)
    {
        if (cam == null)
        {
            Debug.Log("Error: Camera Perspective is not assigned");
            return;
        }
        cam.CameraShake(intensity, duration, fade);
    }

    // set the camera to follow a path determined in the scene THIS MAY END UP BEING VERY CHOPPY. PLEASE TEST
    public IEnumerator CameraFollowPath()
    {
        
        for(int i = 0; i < CamPath.Length; i++)
        {
            yield return new WaitUntil(() => CamCloseEnough(CamPath[i].position));
        }
        yield return new WaitForSeconds(5f);
        cam.target = pm.transform;
    }

    private bool CamCloseEnough(Vector2 tPos)
    {
        Vector2 cPos = cam.transform.position;
        if ((cPos.x <= tPos.x + .1f && cPos.x >= tPos.x - .1f) && (cPos.y <= tPos.y + .1f && cPos.y >= tPos.y - .1f))
            return true;
        return false;
    }

    /* ------ VictoryConditions ------ */

    public abstract bool CheckVictoryCondition();
    
    protected IEnumerator CheckAllRatsDead()
    {
        yield return new WaitUntil(() => enemiesInScene.Count <= 0);
        if (level == 1)
            gm.StartDisplayMessage("kitchen_cleared");
        else
            gm.StartDisplayMessage("get_to_exit");
        rats_dead = true;
        if (CheckVictoryCondition())
            exit.locked = false;
    }

    public virtual void BossDeath()
    {
        boss_dead = true;
        if (CheckVictoryCondition())
            exit.locked = false;
    }
    public void FoundKey()
    {
        gm.StartDisplayMessage("found_key");
        found_key = true;
        if (CheckVictoryCondition())
            exit.locked = false;
    }

    public void CheckPipesClosed() //  PLACEHOLDER
    {
        if (num_pipes_open <= 0)
        {
            gm.StartDisplayMessage("get_to_exit");
            pipes_closed = true;
            if (CheckVictoryCondition())
                exit.locked = false;
        }
    }

    public void ClosedPipe()
    {
        num_pipes_open -= 1;
        CheckPipesClosed();
    }

    /* ------ Level Transitions ------ */

    public void PlayerDeath()
    {
        StopRatMovement();
        gm.respawn = true;
        mm.FadeOutFromFull();
        SceneManager.LoadScene("Death", LoadSceneMode.Single);
    }

    public abstract void LevelComplete();

    public void FadeToBlack()
    {
        black.GetComponent<Animator>().SetBool("Fade", true);
    }

    public IEnumerator FadeToNextLevel()
    {
        black.GetComponent<Animator>().SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneString, LoadSceneMode.Single);
    }
}
