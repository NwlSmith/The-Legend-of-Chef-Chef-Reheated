using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public bool m_on = true;
    public bool paused = false;
    public bool showing_item = false;
    public int cur_level = 1;
    public bool respawn = false;
    public bool debug = false;

    public List<string> inventory = new List<string>();
    private int chef_health = 3;
    private int chef_energy = 8;

    public Dictionary<string, Sprite> shown_info = new Dictionary<string, Sprite>();

    public Sprite[] info_sprites;
    //[(0, Bomb), (1, Carrot), (2, Graters), (3, KnifeSet), (4, Ladles), (5, Pizza), (6, Heart)]

    /* 00. no_energy
     * 01. rats_escaped
     * 02. found_key
     * 03. carrot_message

     * 04. press_close
     * 05. press_open
     * 06. press_push
     * 07. press_unlock
     * 08. basement_unlock
     * 09. go_inside

     * 10. need_weapon
     * 11. kitchen_cleared
     * 12. get_to_exit */

    // this is just a "quality of life" thing
    private Dictionary<string, int> message_key = new Dictionary<string, int>();
    [HideInInspector] public LevelManager levelManager;
    [HideInInspector] public PlayerManager playerManager;
    [HideInInspector] public EnergyPizza energyPizza;
    [HideInInspector] public HealthUI healthUI;
    [HideInInspector] public ItemInfo itemInfo;
    [HideInInspector] public InventoryUI invUI;
    [HideInInspector] public MessageManager messageManager;

    private List<string> respawn_inventory = new List<string>();
    private int respawn_health = 3;
    private int respawn_energy = 8;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        CompileManagers();

        CompileInfoSprites();
        CompileMessageStrings();

        DontDestroyOnLoad(gameObject);
    }

    private void CompileManagers()
    {
        levelManager = LevelManager.instance;
        playerManager = PlayerManager.instance;
        energyPizza = EnergyPizza.instance;
        GameObject go = GameObject.FindWithTag("PlayerHealth");
        if (go != null)
            healthUI = go.GetComponent<HealthUI>();
        itemInfo = ItemInfo.instance;
        invUI = InventoryUI.instance;
        messageManager = MessageManager.instance;
    }

    private void PurgeManagers()
    {
        levelManager = null;
        playerManager = null;
        energyPizza = null;
        healthUI = null;
        itemInfo = null;
        invUI = null;
        messageManager = null;
    }

    private void CompileInfoSprites()
    {
        shown_info.Add("Bomb", info_sprites[0]);
        shown_info.Add("Carrot", info_sprites[1]);
        shown_info.Add("Graters", info_sprites[2]);
        shown_info.Add("KnifeSet", info_sprites[3]);
        shown_info.Add("Ladles", info_sprites[4]);
        shown_info.Add("Pizza", info_sprites[5]);
        shown_info.Add("Heart", info_sprites[6]);
    }

    private void CompileMessageStrings()
    {
        message_key.Add("no_energy", 0);
        message_key.Add("rats_escaped", 1);
        message_key.Add("found_key", 2);
        message_key.Add("carrot_message", 3);
        message_key.Add("press_close", 4);
        message_key.Add("press_open", 5);
        message_key.Add("press_push", 6);
        message_key.Add("press_unlock", 7);
        message_key.Add("door_locked", 8);
        message_key.Add("go_inside", 9);
        message_key.Add("need_weapon", 10);
        message_key.Add("kitchen_cleared", 11);
        message_key.Add("get_to_exit", 12);
    }

    private void Start()
    {
        DisplayHealth();
    }

    /* ------ Level Generation ------ */

    private void InitializeLevel()
    {

    }

    /* ------ Misc. ------ */

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha0))
            debug = !debug;
        if (showing_item && Input.GetButtonUp("Melee"))
        {
            UnpauseGame();
            showing_item = false;
            itemInfo.TakeCurrentImageDown();
        }

        if (Input.GetButtonUp("Pause") && !showing_item && levelManager != null)
        {
            if (!paused)
            {
                PauseGame();
                PauseMenuManager.instance.Pause();
            }
            else
                PauseMenuManager.instance.Resume();
        }
    }

    private void OnLevelFinishedLoadingScene(Scene scene, LoadSceneMode mode)
    {
        PurgeManagers();
        CompileManagers();
        if (levelManager == null) // if there is no levelManager instance in the scene, then it must be a story scene
            return;

        // if the scene transition was because the player died, restore data
        if (respawn)
        {
            Debug.Log("Respawning");
            respawn = false;
            RestoreRespawnData();
        }
        //if the scene transition was a normal level progression, store data
        else
        {
            StoreRespawnData();
            RestoreRespawnData();
        }
        DisplayHealth();
        RestoreInventoryUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoadingScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoadingScene;
    }

    /* ------ Stats ------ */

    public void DisplayHealth()
    {
        chef_health = playerManager.health.GetHealth();
        healthUI.UpdateUI(playerManager.health.GetHealth());
    }

    public void DisplayEnergy()
    {
        chef_energy = playerManager.chef_energy;
        energyPizza.UpdateEnergy(playerManager.chef_energy);
    }

    /* ------ Inventory ------ */

    public void AddItem(string item)
    {
        inventory.Add(item);
        if (item != "Pizza" && item != "Heart")
            invUI.Add(item);

        if (item == "Key")
            levelManager.FoundKey();

        else
        {

            if (item == "Graters")
                playerManager.melee_dmg = 2;
            else if (item == "Carrot")
                levelManager.AdjustLights();

            Sprite itemSprite = shown_info[item];
            // if returns null, then have shown before. If does NOT return null, then must show
            if (itemSprite == null)
                return;
            else
            {
                itemInfo.DisplayItem(itemSprite);
                shown_info[item] = null;
                paused = true;
                showing_item = true;
                PauseGame();
            }
        }
    }

    public void RemoveItem(string item)
    {
        inventory.Remove(item);
        invUI.Remove(item);
    }

    /* ------ Pausing ------ */

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        paused = false;
        Time.timeScale = 1;
    }

    /* ------ Messages ------ */
    public void StartDisplayMessage(string mess_string)
    {
        int key;
        if (message_key.TryGetValue(mess_string, out key))
            StartDisplayMessage(key);
        else
            Debug.Log("Error: Input message key does not exist. Aborting StartDisplayMessage of item " + mess_string);
    }

    public void StartDisplayMessage(int mess_num)
    {
        messageManager.StartDisplay(mess_num);
    }

    public void StopDisplayMessage(string mess_string)
    {
        int key;
        if (message_key.TryGetValue(mess_string, out key))
            StopDisplayMessage(key);
        else
            Debug.Log("Error: Input message key does not exist. Aborting StopDisplayMessage of item " + mess_string);
    }

    public void StopDisplayMessage(int mess_num)
    {
        messageManager.StopDisplay(mess_num);
    }

    public void RemoveLight(Light l)
    {
        levelManager.RemoveLight(l);
    }

    /* ------ Camera Shake ------ */

    public void CameraShake(float intensity, float duration, bool fade)
    {
        levelManager.CameraShake(intensity, duration, fade);
    }

    private void StoreRespawnData()
    {
        respawn_inventory = new List<string>(inventory);
        respawn_health = chef_health;
        respawn_energy = chef_energy;
        //called every level transition
    }

    private void RestoreRespawnData()
    {
        inventory = new List<string>(respawn_inventory);
        playerManager.health.SetHealth(respawn_health);
        playerManager.chef_energy = respawn_energy;
        // possibly could put this in Awake? when go to new level, store that data here.
    }

    private void RestoreInventoryUI()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            string item = inventory[i];
            if (item != "Pizza" && item != "Heart")
                invUI.Add(item);
        }
    }

    public void PlayerDeath()
    {
        levelManager.PlayerDeath();
        //Deal with respawn stuff here.
    }
}
