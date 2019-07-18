using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Manager : EntityManager {

    public bool activated = false;
    public float m_movement_speed = 5f;
    public int starting_health = 15;

    public Rigidbody2D purple_projectile;
    public Rigidbody2D green_projectile;

    public Transform[] nodes;
    public Transform target_node;
    public int player_zone;

    public HealthUI HUI;

    public Item[] drops;

    public AudioClip hurt_sound;
    public AudioClip melee_sound;
    public AudioClip death_sound;

    private AudioSource hurt_source;
    private AudioSource melee_source;
    private AudioSource death_source;

    private WaitForSeconds hurt_time = new WaitForSeconds(1.5f);
    private bool hurt = false;
    public bool in_range = false;

    private Color orange = new Color(1f, .5f, 0f, 1f);
    private Color purple = new Color(.8f, 0f, 1f, 1f);
    private Color green = new Color(0f, 1f, 0f, 1f);
    private Color red = new Color(1f, 0f, 0f, 1f);
    private Color blue = new Color(0f, 0f, 1f, 1f);
    private Color[] colors = new Color[5];

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerManager pm;

    private Coroutine behavior_co;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        hurt_source = rb.AddAS();
        hurt_source.clip = hurt_sound;
        melee_source = rb.AddAS();
        melee_source.pitch = .5f;
        melee_source.clip = melee_sound;
        death_source = rb.AddAS();
        death_source.clip = death_sound;

        m_light = GetComponentInChildren<Light>();
        m_light.color = red;

        health = GetComponent<Health>();
        health.SetHealth(starting_health);

        HUI.maxHealth = starting_health;

        HUI.gameObject.SetActive(false);

        target_node = nodes[1];

        colors[0] = orange;
        colors[1] = purple;
        colors[2] = green;
        colors[3] = red;
        colors[4] = blue;
    }

    private void Start ()
    {
        melee_dmg = 2;
        melee_force = 1000f;
        pixel_offset = .2f;
        anim.SetBool("Idle", true);

        pm = GameManager.instance.playerManager;
    }

    private void Update()
    {
        UpdateSortingOrder();
        if (Input.GetKeyDown(KeyCode.K) && GameManager.instance.debug)
            UpdateHealth(-15);
    }

    public override void UpdateHealth(int num)
    {
        health.UpdateHealth(num);
        HUI.UpdateUI(health.GetHealth());

        if (behavior_co != null)
            StopCoroutine(behavior_co);
        behavior_co = null;

        //change health meter
        if (health.GetHealth() <= 0)
            StartCoroutine(Die());
        else if (num < 0)
            StartCoroutine(EnemyHurt());
        else if (num > 0)
            Debug.Log("Error: Enemy has been healed");
    }

    public void UpdateSortingOrder()
    {
        GetComponentInChildren<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((transform.position.y + pixel_offset) * 100f) * -1;
    }

    private void FixedUpdate() 
    {
        if (!activated)
            return;
        if (!hurt && !attacking)
        {
            if (in_range && can_attack && pm.health.GetHealth() > 0)
            {
                if (behavior_co != null)
                    StopCoroutine(behavior_co);
                behavior_co = StartCoroutine(MeleeAttack());
            }
            else if (can_move)
                Move();
        }
    }

    private void Move()
    {
        float speed = m_movement_speed;
        speed *= Time.fixedDeltaTime;

        transform.position = Vector2.MoveTowards(transform.position, target_node.position, speed);


        anim.SetBool("Left", transform.position.x > target_node.position.x);
        anim.SetBool("Right", transform.position.x < target_node.position.x);
        anim.SetBool("Idle", transform.position.x == target_node.position.x);

        if (transform.position == target_node.position)
        {
            if (can_attack)
            {
                if (behavior_co != null)
                    StopCoroutine(behavior_co);
                behavior_co = StartCoroutine(RangeAttack());
            }
            target_node = nodes[(int)((float)player_zone / 2 - .5f)];
        }
    }

    public override void PlayerInZone(int zone)
    {
        player_zone = zone;
    }

    private void Retarget()
    {
        target_node = nodes[(int)((float)player_zone / 2 - .5f)];
    }
    

    private void OnCollisionStay2D(Collision2D collision) // MUST BE FIXED
    {
        GameObject other = collision.gameObject;

        //outline tags it should not interract with here
        if (other.tag == "Colliders" || other.tag == "Enemy")
            return;

        if (other.tag == "Player")
        {
            if (pm.attacking && can_be_hit)
            { // the player hitting the boss makes the boss pushes the player back
                pm.combat.PlayHitSound();
                UpdateHealth(-pm.melee_dmg);
                pm.UpdateHealth(-1);
                Vector2 hit_dir = pm.rb.position - (Vector2)transform.position - new Vector2(0f, 3f);
                pm.rb.AddForce(hit_dir * melee_force);
            }
            else if (can_attack && pm.health.GetHealth() > 0 /* && pm.can_be_hit */)
                StartCoroutine(MeleeAttack());
        }
    }

    private IEnumerator MeleeAttack()
    {
        attacking = true;
        can_move = false;
        anim.SetTrigger("Orange");
        StartCoroutine(ColorTransition(orange));


        float duration = 1f;
        float elapsed_time = 0f;
        float start_intensity = m_light.intensity;
        float end_intensity = 3f;
        float start_range = m_light.range;
        float end_range = 50f;
        while (elapsed_time < duration)
        {
            m_light.intensity = Mathf.Lerp(start_intensity, end_intensity, (elapsed_time / duration));
            m_light.range = Mathf.Lerp(start_range, end_range, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        if (in_range && pm.health.GetHealth() > 0)
        {
            melee_source.pitch = Random.Range(.75f, .85f);
            melee_source.Play();
            pm.UpdateHealth(-melee_dmg);
            Rigidbody2D prb = pm.GetComponent<Rigidbody2D>();
            Vector2 hit_dir = prb.position - (Vector2)transform.position - new Vector2(0f, 3f);
            prb.AddForce(hit_dir * melee_force);
            GameManager.instance.CameraShake(4f, .75f, true);
        }

        duration = .23f;
        elapsed_time = 0f;
        start_intensity = m_light.intensity;
        end_intensity = 1f;
        start_range = m_light.range;
        end_range = 20f;
        while (elapsed_time < duration)
        {
            m_light.intensity = Mathf.Lerp(start_intensity, end_intensity, (elapsed_time / duration));
            m_light.range = Mathf.Lerp(start_range, end_range, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(.25f);

        attacking = false;
        can_move = true;

        Retarget();

        StartCoroutine(ColorTransition(red));

        behavior_co = null;
    }

    private IEnumerator RangeAttack()
    {
        Rigidbody2D projectile;
        int atk_num = Random.Range(0, 2);
        if (atk_num == 0) // green
        {
            StartCoroutine(ColorTransition(green));
            projectile = green_projectile;
            anim.SetTrigger("Green");
        }
        else
        {
            StartCoroutine(ColorTransition(purple));
            projectile = purple_projectile;
            anim.SetTrigger("Purple");
        }


        attacking = true;
        can_move = false;

        float duration = .85f;
        float elapsed_time = 0f;
        float start_intensity = m_light.intensity;
        float end_intensity = 3f;
        float start_range = m_light.range;
        float end_range = 50f;
        while (elapsed_time < duration)
        {
            m_light.intensity = Mathf.Lerp(start_intensity, end_intensity, (elapsed_time / duration));
            m_light.range = Mathf.Lerp(start_range, end_range, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        Instantiate(projectile, rb.position - new Vector2(0f, -.2f), Quaternion.identity);

        duration = .15f;
        elapsed_time = 0f;
        start_intensity = m_light.intensity;
        end_intensity = 1f;
        start_range = m_light.range;
        end_range = 20f;
        while (elapsed_time < duration)
        {
            m_light.intensity = Mathf.Lerp(start_intensity, end_intensity, (elapsed_time / duration));
            m_light.range = Mathf.Lerp(start_range, end_range, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        attacking = false;
        can_move = true;

        Retarget();

        StartCoroutine(ColorTransition(red));

        behavior_co = null;
    }

    private IEnumerator Die() // MUST BE FIXED
    {

        GameManager.instance.RemoveLight(m_light);


        m_light.color = red;
        m_light.range = 20f;
        m_light.intensity = 1f;

        if (behavior_co != null)
            StopCoroutine(behavior_co);

        // stage 1, enemy hurt and spinning 
        hurt_source.pitch = Random.Range(.95f, 1.05f);
        hurt_source.Play();
        //PLAY DEATH PARTICLES
        can_move = false;
        can_attack = false;
        can_be_hit = false;
        hurt = true;
        UpdateCollisionsWithEnemy();
        // FIX THIS, HAS NO DYING ANIM
        anim.SetTrigger("Hurt");

        anim.SetBool("Die", true);

        rb.constraints = RigidbodyConstraints2D.None;

        LevelManager.instance.cam.target = transform;
        WaitForSeconds ratewait = new WaitForSeconds(.2f);

        Coroutine color_trans_co = null;
        for (int i = 0; i < 7; i++)
        {
            if (color_trans_co != null)
                StopCoroutine(color_trans_co);
            color_trans_co = StartCoroutine(ColorTransition(colors[Random.Range(0, colors.Length)]));
            m_light.intensity += .5f;
            yield return ratewait;
        }

        //anim should be playing by now
        ColorTransition(new Color (0f, .5f, 1f, 1f));
        death_source.Play();

        ratewait = new WaitForSeconds(.111f);
        for (int i = 0; i < 18; i++)
        {
            m_light.intensity -= .25f;
            yield return ratewait;
        }

        for (int i = 0; i < 12; i++)
        {
            Vector3 drop_pos = new Vector3((float)(transform.position.x + (i % 4) - 1.5), transform.position.y - (i / 4) + 2, 0f);
            Instantiate(drops[Random.Range(0, drops.Length)], drop_pos, Quaternion.identity);
        }

        LevelManager.instance.BossDeath();
        LevelManager.instance.cam.target = pm.transform;

        StopAllCoroutines();
        Destroy(gameObject);
    }

    public IEnumerator EnemyHurt() // MUST BE FIXED
    {
        if (behavior_co != null)
            StopCoroutine(behavior_co);
        //play hurt sounds

        m_light.color = red;
        m_light.range = 20f;
        m_light.intensity = 1f;
        hurt_source.pitch = Random.Range(.95f, 1.05f);
        hurt_source.Play();
        anim.SetTrigger("Hurt");
        hurt = true;
        attacking = false;
        can_attack = false;
        can_be_hit = false;
        yield return hurt_time;
        hurt = false;
        can_attack = true;
        can_move = true;
        if (behavior_co != null)
            StopCoroutine(behavior_co);
        behavior_co = StartCoroutine(MeleeAttack());
        Retarget();
        yield return hurt_time;
        can_be_hit = true;
    }

    public void UpdateCollisionsWithEnemy()
    {
        if (!can_be_hit)
            gameObject.layer = LayerMask.NameToLayer("Hurt Entity");
        else
            gameObject.layer = LayerMask.NameToLayer("Boss");
    }

    private IEnumerator ColorTransition(Color target)
    {
        float duration = .2f;
        float elapsed_time = 0f;
        Color start_color = m_light.color;
        while (elapsed_time < duration)
        {
            m_light.color = Color.Lerp(start_color, target, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }
    }

}
