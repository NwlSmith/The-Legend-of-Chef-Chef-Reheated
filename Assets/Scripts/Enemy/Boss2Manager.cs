using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Manager : EntityManager
{
    public float m_movement_speed = 3f;
    public int starting_health = 15;

    public bool stage2 = false;

    public Fireball fb1;

    public Transform[] nodes;
    public Transform target_node;
    public int player_zone;

    public HealthUI HUI;

    public AudioClip[] hurt_sounds;
    public AudioClip summon_sound;
    public AudioClip[] transition_sounds;

    public FireballSpawn[] g0;
    public FireballSpawn[] g1;
    public FireballSpawn[] g2;
    public FireballSpawn[] g3;
    public FireballSpawn[] g4;
    public FireballSpawn[] g5;
    private List<FireballSpawn[]> fb_spawn_list = new List<FireballSpawn[]>();

    private AudioSource hurt_source;
    private AudioSource summon_source;
    private AudioSource transition_source;

    private readonly WaitForSeconds half_atk_time = new WaitForSeconds(.5f);
    private readonly WaitForSeconds hurt_time = new WaitForSeconds(1.625f);

    private Rigidbody2D rb;
    private Animator anim;
    private Animator light_anim;
    private PlayerManager pm;
    private LevelManager lm;

    private Coroutine move_co;
    private Coroutine atk_co;
    private Coroutine atk_countdown_co;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        light_anim = transform.GetChild(2).GetComponent<Animator>();

        fb_spawn_list.Add(g0);
        fb_spawn_list.Add(g1);
        fb_spawn_list.Add(g2);
        fb_spawn_list.Add(g3);
        fb_spawn_list.Add(g4);
        fb_spawn_list.Add(g5);

        hurt_source = rb.AddAS();
        hurt_source.pitch = .5f;
        summon_source = rb.AddAS();
        summon_source.clip = summon_sound;
        transition_source = rb.AddAS();
        transition_source.clip = transition_sounds[0];

        m_light = GetComponentInChildren<Light>();

        health = GetComponent<Health>();
        health.SetHealth(starting_health);

        HUI.maxHealth = starting_health;

        target_node = nodes[1];
    }

    private void Start()
    {
        melee_dmg = 2;
        melee_force = 1000f;
        pixel_offset = .2f;
        anim.SetBool("Idle", true);

        pm = GameManager.instance.playerManager;
        lm = GameManager.instance.levelManager;

        move_co = StartCoroutine(Move());
    }

    private void Update()
    {
        UpdateSortingOrder();
        if (Input.GetKeyDown(KeyCode.K) && GameManager.instance.debug)
            UpdateHealth(-15);
    }

    public void UpdateSortingOrder()
    {
        GetComponentInChildren<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((transform.position.y + pixel_offset) * 100f) * -1;
    }

    private void FixedUpdate()
    {
        if (move_co == null && can_move)
            move_co = StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        anim.SetBool("Left", transform.position.x > target_node.position.x);
        anim.SetBool("Right", transform.position.x < target_node.position.x);
        anim.SetBool("Down", transform.position.y > target_node.position.y);
        anim.SetBool("Up", transform.position.y < target_node.position.y);

        if (atk_countdown_co != null)
            StopCoroutine(atk_countdown_co);
        atk_countdown_co = StartCoroutine(AtkCountdown());

        // may need to rework this
        while (transform.position != target_node.position)
        {
            float speed = m_movement_speed;
            speed *= Time.fixedDeltaTime;
            

            transform.position = Vector2.MoveTowards(transform.position, target_node.position, speed);
            
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("Arrived at target. Player in zone " + player_zone);

        // when get to destination, stop countdown and just attack. encourages the chef to not stay in one zone? This will rapid fire though.
        if (atk_countdown_co != null)
            StopCoroutine(atk_countdown_co);
        atk_countdown_co = StartCoroutine(AtkCountdown());
        yield return hurt_time;
        RandomAttack();
        Retarget();
    }

    public override void PlayerInZone(int zone)
    {
        player_zone = zone;
    }

    //redo this, have go to node 2, then player node, not just right to player node.
    private void Retarget()
    {
        if (target_node != nodes[player_zone - 1] && transform.position != nodes[1].position)
            target_node = nodes[1];
        else
            target_node = nodes[player_zone - 1];
        
        Debug.Log("Retargeting, target now at " + target_node.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pm.combat.PlayHitSound();
            UpdateHealth(-pm.melee_dmg);
            pm.UpdateHealth(-1);
            Vector2 hit_dir = pm.rb.position - (Vector2)transform.position;
            pm.rb.AddForce(hit_dir * melee_force);
        }
    }


    private IEnumerator AtkCountdown()
    {
        if (stage2)
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        else
            yield return new WaitForSeconds(Random.Range(7.5f, 12.5f));

        RandomAttack();
    }

    private void RandomAttack()
    {
        Attack(Random.Range(1, 4));
    }

    private void Attack(int num)
    {
        if (can_attack == false)
        {
            Debug.Log("Error: Attack called, but boss cannot attack.");
            return;
        }

        if (atk_co != null)
        {
            Debug.Log("Error: Attack coroutine is not null");
            return;
        }

        anim.SetTrigger("Summon");
        string flash_light_trigger = null;
        switch (num)
        {
            case 1:
                atk_co = StartCoroutine(Atk1());
                flash_light_trigger = "Blue";
                break;
            case 2:
                atk_co = StartCoroutine(Atk2());
                flash_light_trigger = "Yellow";
                break;
            case 3:
                atk_co = StartCoroutine(Atk3());
                flash_light_trigger = "Purple";
                break;
            case 4:
                atk_co = StartCoroutine(Atk4());
                flash_light_trigger = "Red";
                break;
            default:
                Debug.Log("Error: Attack number " + num + " is invalid");
                return;
        }
        light_anim.SetTrigger(flash_light_trigger);
        summon_source.pitch = Random.Range(.45f, .55f);
        summon_source.Play();
        can_attack = false;
        attacking = true;
        can_move = false;
        if (move_co != null)
            StopCoroutine(move_co);
        if (num == 2)
            can_be_hit = false;
    }

    private IEnumerator Atk1()
    {
        yield return half_atk_time;
        lm.SpawnObjects();
        yield return half_atk_time;
        yield return hurt_time;
        ResetBoss();
    }

    private IEnumerator Atk2()
    {
        yield return half_atk_time;

        SpawnFireballInZone(0);

        if (stage2)
            SpawnFireballInZone(player_zone);

        yield return half_atk_time;

        yield return hurt_time;

        ResetBoss();

        Attack(4);
    }

    private IEnumerator Atk3()
    {
        yield return half_atk_time;

        SpawnFireballInZone(player_zone);

        yield return half_atk_time;

        yield return hurt_time;

        ResetBoss();
    }

    private IEnumerator Atk4()
    {
        can_be_hit = false;
        // if stage1, shoot 3 times, if stage2, fire 5 times
        int t = 2;
        WaitForSeconds timestep = new WaitForSeconds(.5f);
        if (stage2)
        {
            t = 4;
            timestep = new WaitForSeconds(.25f);
        }

        for (int i = 0; i < t; i++)
        {
            ClosestFBS(g0).Fire1();
            yield return timestep;
        }
        ClosestFBS(g0).Fire1();
        yield return hurt_time;

        ResetBoss();
    }

    private void SpawnFireballInZone(int num)
    {
        foreach (FireballSpawn fbs in fb_spawn_list[num])
            fbs.Fire();
    }

    private FireballSpawn ClosestFBS(FireballSpawn[] g)
    {
        FireballSpawn closest_fbs = g[0];
        foreach (FireballSpawn fbs in g)
            if (Vector3.Distance(fbs.transform.position, pm.transform.position) <
                Vector3.Distance(closest_fbs.transform.position, pm.transform.position))
                closest_fbs = fbs;
        return closest_fbs;
    }

    private void ResetBoss()
    {
        if (atk_co != null)
            atk_co = null;

        can_attack = true;
        attacking = false;
        can_move = true;
        can_be_hit = true;
        if (move_co != null)
            StopCoroutine(move_co);
        move_co = StartCoroutine(Move());
    }



    public override void UpdateHealth(int num)
    {
        if (!can_be_hit)
            return;
        health.UpdateHealth(num);
        HUI.UpdateUI(health.GetHealth());

        //change health meter
        if (health.GetHealth() <= 0)
        {
            if (stage2)
            {
                StopAllCoroutines();
                StartCoroutine(Die());
            }
            else
                StartCoroutine(Transform());
        }
        else if (num < 0)
            StartCoroutine(EnemyHurt());
        else if (num > 0)
            Debug.Log("Error: Enemy has been healed");
    }

    private IEnumerator Die()
    {
        UpdateCollisionsWithEnemy();

        StopBehaviorCoroutines();
        lm.KillAllRats();
        DetonateAllFireballs();

        can_attack = false;
        can_move = false;
        attacking = false;
        can_be_hit = false;
        anim.SetTrigger("Die");
        light_anim.SetTrigger("Die");
        lm.cam.target = transform;
        transition_source.pitch = 1f;
        transition_source.clip = transition_sounds[1];
        transition_source.Play();


        yield return new WaitForSeconds(5f);
        lm.BossDeath();

    }

    public void UpdateCollisionsWithEnemy()
    {
        if (!can_be_hit)
            gameObject.layer = LayerMask.NameToLayer("Hurt Entity");
        else
            gameObject.layer = LayerMask.NameToLayer("Boss");
    }

    private IEnumerator Transform()
    {

        lm.KillAllRats();

        lm.cam.target = transform;

        UpdateCollisionsWithEnemy();

        StopBehaviorCoroutines();

        can_attack = false;
        can_move = false;
        attacking = false;
        can_be_hit = false;
        anim.SetTrigger("Transform");
        light_anim.SetTrigger("Transform");
        SetSpawnersStage2();
        lm.KillAllRats();
        DetonateAllFireballs();
        transition_source.pitch = .75f;
        transition_source.Play();

        yield return new WaitForSeconds(2.125f);

        GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 2.5f);

        lm.cam.target = pm.transform;

        stage2 = true;
        anim.SetBool("Stage_2", true);

        health.SetHealth(starting_health);
        HUI.UpdateUI(health.GetHealth());
        m_movement_speed = 5f;
        ResetBoss();
        UpdateCollisionsWithEnemy();
    }

    private void DetonateAllFireballs()
    {
        GameObject[] fbs = GameObject.FindGameObjectsWithTag("BossProjectile");
        foreach (GameObject fb in fbs)
        {
            Fireball script = fb.GetComponent<Fireball>();
            if (script != null)
                script.Explode();
        }
    }

    private void SetSpawnersStage2()
    {
        foreach (FireballSpawn fbs in g1)
            fbs.Stage2();
        foreach (FireballSpawn fbs in g2)
            fbs.Stage2();
        foreach (FireballSpawn fbs in g3)
            fbs.Stage2();
        foreach (FireballSpawn fbs in g4)
            fbs.Stage2();
        foreach (FireballSpawn fbs in g5)
            fbs.Stage2();
    }

    private IEnumerator EnemyHurt()
    {
        UpdateCollisionsWithEnemy();
        StopBehaviorCoroutines();
        hurt_source.clip = hurt_sounds[Random.Range(0, hurt_sounds.Length)];
        hurt_source.Play();
        anim.SetTrigger("Hurt");
        light_anim.SetTrigger("Hurt");
        can_be_hit = false;
        yield return hurt_time;
        ResetBoss();
        UpdateCollisionsWithEnemy();
    }

    public void Retaliate()
    {
        if (health.GetHealth() > 0)
            StartCoroutine(Atk4());
    }

    private void StopBehaviorCoroutines()
    {
        if (move_co != null)
            StopCoroutine(move_co);
        if (atk_co != null)
            StopCoroutine(atk_co);
        if (atk_countdown_co != null)
            StopCoroutine(atk_countdown_co);
    }
}
