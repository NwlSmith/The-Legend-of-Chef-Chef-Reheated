using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : EntityManager
{
    public float pixel_offset_from_bottom = -.65f;
    public float m_movement_speed = 3f;
    public int m_melee_damage = 1;
    public float m_melee_force = 6000f;
    public int m_enemy_starting_health = 3;
    public float m_hurt_volume = .5f;
    public float m_enemy_hurt_time_float = .5f;
    public float m_enemy_hit_time_float = .5f;
    public float m_die_torque = 5000f;
    public float m_sound_pitch = 1f;

    public AudioClip[] m_hit_sounds;
    public AudioClip[] m_hurt_sounds;
    public AudioClip[] m_death_sounds;

    public HealthUI HUI;

    public Transform m_spawn_point;

    public int chance_of_drops = 2; // as in, the chance of getting a drop is one in chance_of_drops+1
    public Item[] drops;

    [HideInInspector] public AudioSource hit_source;
    [HideInInspector] public AudioSource hurt_source;
    [HideInInspector] public AudioSource death_source;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;

    [HideInInspector] public Pathfinding.IAstarAI m_path;
    [HideInInspector] public Pathfinding.AIDestinationSetter m_dest;
    [HideInInspector] public PlayerManager pm;

    protected WaitForSeconds enemy_hurt_time;
    protected WaitForSeconds enemy_hit_time;

    protected float m_last_x;
    protected float m_last_y;
    protected bool m_moving_down = false;
    protected bool m_moving_up = false;
    protected bool m_moving_right = false;
    protected bool m_moving_left = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren <Animator>();

        m_light = GetComponentInChildren<Light>();

        health = GetComponent<Health>();
        health.SetHealth(m_enemy_starting_health);

        HUI.maxHealth = m_enemy_starting_health;

        melee_dmg = m_melee_damage;
        melee_force = m_melee_force;
        pixel_offset = pixel_offset_from_bottom;

        hit_source = rb.AddAS();
        hurt_source = rb.AddAS();
        death_source = rb.AddAS();

        hurt_source.volume = m_hurt_volume;

        death_source.clip = m_death_sounds[Random.Range(0, m_death_sounds.Length)];

        hit_source.pitch = m_sound_pitch * 2;
        hurt_source.pitch = m_sound_pitch;
        death_source.pitch = m_sound_pitch;

        enemy_hurt_time = new WaitForSeconds(m_enemy_hurt_time_float);
        enemy_hit_time = new WaitForSeconds(m_enemy_hit_time_float);

        m_path = GetComponent<Pathfinding.IAstarAI>();
        m_path.maxSpeed = m_movement_speed;

        m_dest = GetComponent<Pathfinding.AIDestinationSetter>();

        can_attack = true;
    }

    protected virtual void Start()
    {
        m_last_x = transform.position.x;
        m_last_y = transform.position.y;

        pm = GameManager.instance.playerManager;

        m_spawn_point = transform;

        StartCoroutine(SpawnEnemy());
    }

    protected IEnumerator SpawnEnemy()
    {
        Vector3 start_scale = transform.localScale = new Vector3(0f, 0f, 1f);
        float elapsed_time = 0f;
        float growlifetime = .25f;
        Vector3 end_scale = new Vector3(1f, 1f, 1f);

        while (elapsed_time < growlifetime)
        {
            transform.localScale = Vector3.Lerp(start_scale, end_scale, elapsed_time / growlifetime);
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = end_scale;
    }

    protected void Update()
    {
        UpdateSortingOrder();
    }

    protected void FixedUpdate()
    {
        if (!m_path.canMove)
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0f, 0f), .1f);
        else
            HandleAnimations();

        if (pm != null && (Vector3.Distance(transform.position, pm.transform.position) > 7 || pm.health.GetHealth() <= 0))
            m_path.canMove = false;
        else
            m_path.canMove = true;
    }

    protected void UpdateSortingOrder()
    {
        GetComponentInChildren<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((transform.position.y + pixel_offset) * 100f) * -1;
    }

    protected void HandleAnimations()
    {
        float new_x = transform.position.x;
        float new_y = transform.position.y;

        float moveX = new_x - m_last_x;
        float moveY = new_y - m_last_y;

        //angle relative to x axis
        float tan = Mathf.Atan2(moveY, moveX);

        if (tan > -0.7854 && tan < 0.7854)
            m_moving_right = true;
        else
            m_moving_right = false;
        anim.SetBool("Moving_Right", m_moving_right);

        if (tan > 0.7854 && tan < 2.3562)
            m_moving_up = true;
        else
            m_moving_up = false;
        anim.SetBool("Moving_Up", m_moving_up);

        if (tan > 2.3562 || tan < -2.3562)
            m_moving_left = true;
        else
            m_moving_left = false;
        anim.SetBool("Moving_Left", m_moving_left);
        

        if (tan > -2.3562  && tan < -0.7854)
            m_moving_down = true;
        else
            m_moving_down = false;
        anim.SetBool("Moving_Down", m_moving_down);

        m_last_x = new_x;
        m_last_y = new_y;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        //outline tags it should not interract with here
        if (other.tag == "Colliders" || other.tag == "Enemy")
            return;

        EntityManager em = other.GetComponent<EntityManager>();
        if (em == null)
        {
            Debug.Log("GameObject has no EntityManager");
        }
        else if (em.gameObject.tag == "Player")
        {
            PlayerManager pm = other.GetComponent<PlayerManager>();
            if (pm.health.GetHealth() > 0)
                StartCoroutine(MeleeAttack(pm));
        }
    }

    protected IEnumerator MeleeAttack(PlayerManager player)
    {
        // if chef attacking, then you take damage,
        // if he isn't attacking or dizzy, he takes damage

        GameManager.instance.CameraShake(2f, .75f, true);

        // Player hitting enemy
        if (player.attacking && can_be_hit)
        {
            player.combat.PlayHitSound();
            UpdateHealth(-player.melee_dmg);
            Vector2 hit_dir = rb.position - (Vector2)player.transform.position;
            rb.AddForce(hit_dir * player.melee_force);
        }
        // Enemy hitting Player
        else if (!player.attacking && can_attack && player.can_be_hit)
        {
            player.UpdateHealth(-melee_dmg);
            Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
            Vector2 hit_dir = prb.position - (Vector2)transform.position;
            prb.AddForce(hit_dir * melee_force);

            can_attack = false;

            hit_source.clip = m_hit_sounds[Random.Range(0, m_hit_sounds.Length)];
            hit_source.Play();
            anim.SetTrigger("Attack");
            m_path.canMove = false;
            can_attack = false;
            yield return enemy_hit_time;
            m_path.canMove = true;
            can_attack = true;
        }
    }

    public override void UpdateHealth(int num)
    {
        //Debug.Log("Updating Rat Health, current health  = " + health.GetHealth() + " num = " + num);
        health.UpdateHealth(num);
        HUI.UpdateUI(health.GetHealth());
        
        //change health meter
        if (health.GetHealth() <= 0)
            StartCoroutine(Die());
        else if (num < 0)
            StartCoroutine(EnemyHurt());
        else if (num > 0)
            Debug.Log("Error: Enemy has been healed");
    }

    protected virtual IEnumerator Die()
    {

        GameManager.instance.RemoveLight(m_light);

        // stage 1, enemy hurt and spinning 
        hurt_source.clip = m_hurt_sounds[Random.Range(0, m_hurt_sounds.Length - 1)];
        hurt_source.Play();
        //PLAY DEATH PARTICLES
        m_path.canMove = false;
        can_attack = false;
        can_be_hit = false;
        UpdateCollisionsWithEnemy();
        // FIX THIS, HAS NO DYING ANIM
        anim.SetTrigger("Hurt");

        rb.constraints = RigidbodyConstraints2D.None;

        StartCoroutine(ExtinguishLight());

        ConstantForce2D torque = GetComponent<ConstantForce2D>();
        torque.torque = m_die_torque; // 10f
        float elapsed_time = 0f;
        Vector3 start_scale = new Vector3(1f, 1f, 1f);
        Vector3 start_pos = transform.position;
        Vector3 target_scale = new Vector3(0f, 0f, 1f);
        Vector3 target_pos = transform.position - new Vector3(0f, -.25f, 0f);
        float duration = .5f;
        while (elapsed_time < duration)
        {
            float timestep = elapsed_time / duration;
            transform.localScale = Vector3.Lerp(start_scale, target_scale, timestep);
            transform.position = Vector3.Lerp(start_pos, target_pos, timestep);
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        anim.transform.localScale = new Vector3(.25f, .25f, .25f);

        anim.SetTrigger("Die");
        death_source.Play();

        ExtinguishLight();

        yield return enemy_hurt_time;

        if (Random.Range(0, chance_of_drops) <= 0)
            Instantiate(drops[Random.Range(0, drops.Length)], transform.position, Quaternion.identity);
        GameManager.instance.levelManager.enemiesInScene.Remove(this);

        StopAllCoroutines();
        Destroy(gameObject);
    }

    protected IEnumerator EnemyHurt()
    {
        hurt_source.clip = m_hurt_sounds[Random.Range(0, m_hurt_sounds.Length - 1)];
        hurt_source.Play();
        anim.SetTrigger("Hurt");
        m_path.canMove = false;
        can_attack = false;
        can_be_hit = false;
        UpdateCollisionsWithEnemy();
        //enemy_hurt = true;
        yield return enemy_hurt_time;
        m_path.canMove = true;
        can_attack = true;
        rb.velocity = Vector2.zero;
        yield return enemy_hurt_time;
        can_be_hit = true;
        UpdateCollisionsWithEnemy();
        //enemy_hurt = false;        
    }

    public void UpdateCollisionsWithEnemy()
    {
        if (!can_be_hit)
            gameObject.layer = LayerMask.NameToLayer("Hurt Entity");
        else
            gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public void StopMoving()
    {
        can_attack = false;
        if (m_path != null)
            m_path.canMove = false;
    }

    public void GoToSpawn()
    {
        m_dest.target = m_spawn_point;
    }

}
