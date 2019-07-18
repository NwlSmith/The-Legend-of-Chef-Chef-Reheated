using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat3Manager : EnemyManager {

    public LayerMask boss_mask;
    public LayerMask player_mask;
    public LayerMask enemy_mask;
    public LayerMask hurt_entity_mask;
    private LayerMask[] target_masks;

    //hitsounds are used instead of booms
    public AudioClip beep_sound;
    private AudioSource beep_source;

    private Light green;
    private Light red;

    private readonly float m_explosion_radius = 2f;
    private readonly float m_explosion_force = 750f;
    private readonly float m_max_damage = 2f;

    private Coroutine beep_co;

    protected override void Awake()
    {
        base.Awake();
        green = transform.GetChild(2).GetComponent<Light>();
        red = transform.GetChild(3).GetComponent<Light>();

        target_masks = new LayerMask[4];
        target_masks[0] = boss_mask;
        target_masks[1] = player_mask;
        target_masks[2] = enemy_mask;
        target_masks[3] = hurt_entity_mask;

        beep_source = rb.AddAS();
        beep_source.clip = beep_sound;
        beep_source.volume = .005f;
    }

    protected override void Start()
    {
        base.Start();
        beep_co = StartCoroutine(Beep());
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;


        if (health.GetHealth() > 0 && (other.tag == "Player" || other.tag == "BossProjectile" || other.tag == "Knife"))
        {
            //have the bomb detonate
            rb.velocity = 0f * transform.up;

            GetComponent<Collider2D>().enabled = false;
            rb.angularVelocity = 0f;
            m_path.canMove = can_move = false;

            UpdateHealth(-1);

        }
    }

    protected IEnumerator Beep()
    {
        while (true)
        {
            if (Vector2.Distance(pm.transform.position, transform.position) < 10)
                beep_source.Play();
            yield return new WaitForSeconds(1f);
        }
    }

    private void Explode()
    {
        for (int i = 0; i < target_masks.Length; i++)
            DamageNearby(target_masks[i]);

        if (beep_co != null)
            StopCoroutine(beep_co);
        hit_source.clip = m_hit_sounds[Random.Range(0, m_hit_sounds.Length)];
        hit_source.Play();

        green.GetComponent<Animator>().SetTrigger("Explode");
        red.enabled = false;

        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(true);
        transform.GetChild(4).gameObject.GetComponent<Animator>().SetTrigger("explode");

        GameManager.instance.CameraShake(7.5f, 1f, true);


        Destroy(transform.GetChild(4).gameObject, .51f);
        Destroy(gameObject, 1f);
    }

    private void DamageNearby(LayerMask lm)
    {
        // Find all the enemies in an area around the bomb and damage them.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_explosion_radius, lm);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody2D targetRigidbody = colliders[i].GetComponent<Rigidbody2D>();
            if (!targetRigidbody)
                continue;
            if (GameManager.instance.debug)
                Debug.Log("target name: " + targetRigidbody.name);

            if (targetRigidbody.name != name && targetRigidbody.transform.position != transform.position)
            {
                AddExplosionForce(targetRigidbody);

                targetRigidbody.GetComponent<EntityManager>().UpdateHealth(CalculateDamage(targetRigidbody.position));
            }
        }
    }

    private void AddExplosionForce(Rigidbody2D rb)
    {
        var explosion_dir = rb.position - (Vector2)transform.position;
        var explosion_dist = explosion_dir.magnitude;

        explosion_dir /= explosion_dist;

        rb.AddExplosionForce(m_explosion_force, transform.position, m_explosion_radius);
    }

    private int CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on its position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_explosion_radius - explosionDistance) / m_explosion_radius;

        float damage = relativeDistance * m_max_damage;

        damage = Mathf.Max(0f, damage);

        return (int)-damage;
    }


    protected override IEnumerator Die()
    {
        // stage 1, enemy hurt and spinning 
        hurt_source.clip = m_hurt_sounds[Random.Range(0, m_hurt_sounds.Length - 1)];
        hurt_source.Play();
        //PLAY DEATH PARTICLES
        m_path.canMove = false;
        can_move = false;
        can_attack = false;
        can_be_hit = false;
        UpdateCollisionsWithEnemy();
        // FIX THIS, HAS NO DYING ANIM
        death_source.Play();

        Explode();

        yield return enemy_hurt_time;

        if (Random.Range(0, chance_of_drops) <= 0)
            Instantiate(drops[Random.Range(0, drops.Length)], transform.position, Quaternion.identity);
        GameManager.instance.levelManager.enemiesInScene.Remove(this);

        StopAllCoroutines();
        Destroy(gameObject, 1f);
    }
}
