using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : EntityManager {

    public static PlayerManager instance = null;

    public float m_melee_time_float = 1f;
    public float m_energy_regen_time_float = 1.5f;
    public float m_chef_hurt_time_float = 1.5f;
    public float m_chef_dizzy_time_float = 1f;

    public float pixel_offset_from_bottom = -.8f;
    public float m_normal_speed = 4f;
    public float m_dash_speed = 8f;
    public float m_speed = 4f;
    public float knife_speed = 20f;
    public int chef_energy = 8;
    public int chef_starting_health = 3;
    public int chef_max_health = 10;
    public int chef_melee_dmg = 1;
    public float chef_melee_force = 1000f;
    //public int chef_health = 3;
    public bool chef_dizzy = false;
    public bool chef_throwing = false; // NEED TO SET THIS TO FALSE

    public AudioClip[] melee_attack_sounds;
    public AudioClip[] hurt_sounds;
    public AudioClip[] knife_throw_sounds;
    public AudioClip death_sound;
    public AudioClip dizzy_sound;
    public AudioClip walk_sound;
    // CHEF SHOULD NOT HAVE THIS, ENEMY SHOULD
    public AudioClip[] ladle_hit_sounds;
    public AudioClip grater_hit_sound;

    public Rigidbody2D[] knives;
    public GameObject bomb;
    public Queue<GameObject> bombs = new Queue<GameObject>();

    public HealthUI HUI;

    [HideInInspector] public AudioSource melee_attack_source;
    [HideInInspector] public AudioSource hit_source;
    [HideInInspector] public AudioSource hurt_source;
    [HideInInspector] public AudioSource knife_throw_source;
    [HideInInspector] public AudioSource death_source;
    [HideInInspector] public AudioSource dizzy_source;
    [HideInInspector] public AudioSource walk_source;


    [HideInInspector] public WaitForSeconds melee_time;
    [HideInInspector] public WaitForSeconds energy_regen_time;
    [HideInInspector] public WaitForSeconds chef_hurt_time;
    [HideInInspector] public WaitForSeconds chef_dizzy_time;
    [HideInInspector] public WaitForSeconds quarter_second;

    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerCombat combat;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;

    [HideInInspector] public GameManager gm;

    private Coroutine death_co = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        m_light = GetComponentInChildren<Light>();

        melee_time = new WaitForSeconds(m_melee_time_float);
        energy_regen_time = new WaitForSeconds(m_energy_regen_time_float);
        chef_hurt_time = new WaitForSeconds(m_chef_hurt_time_float);
        chef_dizzy_time = new WaitForSeconds(m_chef_dizzy_time_float);
        quarter_second = new WaitForSeconds(.25f);

        melee_dmg = chef_melee_dmg;
        melee_force = chef_melee_force;
        pixel_offset = pixel_offset_from_bottom;

        melee_attack_source = rb.AddAS();
        hit_source = rb.AddAS();
        hurt_source = rb.AddAS();
        knife_throw_source = rb.AddAS();

        death_source = rb.AddAS();
        dizzy_source = rb.AddAS();
        death_source.clip = death_sound;
        dizzy_source.clip = dizzy_sound;

        walk_source = rb.AddAS();
        walk_source.clip = walk_sound;
        walk_source.loop = true;
        walk_source.volume = .5f;

        Physics2D.IgnoreLayerCollision(10, 8, false);

        health = GetComponent<Health>();
        health.SetHealth(chef_starting_health);

        HUI.maxHealth = chef_max_health;
    }

    private void Start()
    {
        gm = GameManager.instance;
        gm.DisplayEnergy();
    }

    private void Update()
    {
        // if the game is paused, don't take input.
        if (gm.paused)
            return;

        UpdateSortingOrder();
    }

    public void UpdateSortingOrder()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((transform.position.y + pixel_offset) * 100f) * -1;
    }

    // Intermediary between item and gamemanager. This is apparently useless... But could be useful bc it allows for
    //the possibility that 2 players could have indipendant inventories?
    public void AddItem(string item)
    {
        gm.AddItem(item);
    }

    public void UpdateEnergy(int num)
    {
        if (chef_energy == 8 && num > 0)
            return;
        chef_energy += num;
        //change energy meter

        gm.DisplayEnergy();
    }

    public override void UpdateHealth(int num)
    {
        if (gm.debug)
            Debug.Log("Updating Chef Health, current health  = " + health.GetHealth() + " num = " + num);
        health.UpdateHealth(num);
        //change health meter
        HUI.UpdateUI(health.GetHealth());

        gm.DisplayHealth();

        if (health.GetHealth() <= 0)
        {
            if (death_co == null)
                death_co = StartCoroutine(Die());
        }
        else if (num < 0)
            StartCoroutine(ChefHurt());
        else if (num > 0)
        {
            hurt_source.clip = hurt_sounds[hurt_sounds.Length - 1];
            hurt_source.Play();
        }
    }

    private IEnumerator Die()
    {
        can_move = false;
        can_attack = false;
        can_be_hit = false;
        anim.SetTrigger("Dying");
        UpdateCollisionsWithEnemy();
        //Tell enemies to stop attacking ---------------------------------------------
        for (int i = 0; i < 4; i++)
        {
            if (i == 3)
                gm.levelManager.FadeToBlack();
            death_source.Play();
            yield return new WaitForSeconds(1f);
            death_source.pitch -= .05f;
        }

        gm.PlayerDeath();
        StopAllCoroutines();
    }

    public IEnumerator ChefHurt()
    {
        if (gm.debug)
            Debug.Log("Chef has been hurt!");
        gm.CameraShake(5f, 1f, true);
        hurt_source.clip = hurt_sounds[Random.Range(0, hurt_sounds.Length - 1)];
        hurt_source.Play();
        can_be_hit = false;
        StartCoroutine(ChefDizzy());
        UpdateCollisionsWithEnemy();
        yield return chef_hurt_time;
        can_be_hit = true;
        UpdateCollisionsWithEnemy();
    }

    public void UpdateCollisionsWithEnemy()
    {
        Physics2D.IgnoreLayerCollision(10, 8, !can_be_hit);
    }

    public IEnumerator ChefDizzy()
    {
        if (health.GetHealth() <= 0)
        {
            can_move = false;
            can_attack = false;
            can_be_hit = false;
            anim.SetTrigger("Dying");
            death_source.Play();
        }
        else
        {
            anim.SetTrigger("Hurt");
            chef_dizzy = true;
            dizzy_source.Play();
            yield return chef_dizzy_time;
            chef_dizzy = false;
            rb.velocity = Vector2.zero;
        }
    }

    public void ResetValues()
    {
        chef_energy = 8;
        chef_melee_dmg = 1;
        chef_dizzy = false;
        chef_throwing = false;
        can_be_hit = true;

        Physics2D.IgnoreLayerCollision(10, 8, false);

        movement.ResetValues();
    }
}
