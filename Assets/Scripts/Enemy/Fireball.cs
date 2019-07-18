using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Fireball : MonoBehaviour {

    public float m_explosion_radius = 2f;
    public float m_explosion_force = 500f;
    public float m_max_damage = 2f;

    public float m_speed = 3f;
    protected readonly float m_lifetime = 10f;

    public LayerMask player_mask;
    public LayerMask enemy_mask;
    public LayerMask hurt_entity_mask;
    public LayerMask boss_mask;
    private LayerMask[] target_masks;

    public AudioClip throw_sound;
    public AudioClip[] booms;
    private AudioSource throw_source;
    private AudioSource boom_source;
    protected Rigidbody2D rb;
    private SpriteRenderer sr;
    private GameObject explosion;
    private Light l;
    private Color orange = new Color(1f, .5f, 0f, 1f);
    private Coroutine color_change_co;
    private Coroutine shrink_co;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        explosion = transform.GetChild(0).gameObject;
        l = transform.GetChild(1).GetComponent<Light>();

        throw_source = rb.AddAS();
        throw_source.clip = throw_sound;
        throw_source.pitch = Random.Range(.08f, .12f);
        throw_source.volume = .5f;
        boom_source = rb.AddAS();
        boom_source.clip = booms[Random.Range(0, booms.Length)];

        target_masks = new LayerMask[4];
        target_masks[0] = player_mask;
        target_masks[1] = enemy_mask;
        target_masks[2] = hurt_entity_mask;
        target_masks[3] = boss_mask;
    }

    protected virtual void Start()
    {
        StartCoroutine(CreateProjectile());
        throw_source.Play();
        color_change_co = StartCoroutine(ChangeColor());
        StartMovement();

        GameManager.instance.CameraShake(2.5f, .5f, true);
    }

    private IEnumerator CreateProjectile()
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

        shrink_co = StartCoroutine(ShrinkProjectile());
    }

    private IEnumerator ShrinkProjectile()
    {
        yield return new WaitForSeconds(50f);

        Vector3 start_scale = transform.localScale = new Vector3(1f, 1f, 1f);
        float elapsed_time = 0f;

        Vector3 end_scale = Vector3.zero;
        float start_volume = throw_source.volume;

        while (elapsed_time < m_lifetime)
        {
            float timescale = elapsed_time / m_lifetime;
            transform.localScale = Vector3.Lerp(start_scale, end_scale, timescale);
            throw_source.volume = Mathf.Lerp(start_volume, 0f, timescale);
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    protected abstract void StartMovement();
    protected abstract void StopMovement();

    private void OnTriggerEnter2D(Collider2D other)
    {
        //have the fireball detonate


        StopMovement();
        if (shrink_co != null)
            StopCoroutine(shrink_co);
        rb.velocity = 0f * transform.up;
        rb.angularVelocity = 0f;

        Explode();

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
    }

    public void Explode()
    {
        for (int i = 0; i < target_masks.Length; i++)
            DamageNearby(target_masks[i]);

        boom_source.Play();

        if (color_change_co != null)
            StopCoroutine(color_change_co);
        StartCoroutine(LightFlash());

        sr.enabled = false;
        explosion.SetActive(true);
        explosion.GetComponent<Animator>().SetTrigger("explode");

        GameManager.instance.CameraShake(7.5f, 1f, true);


        Destroy(explosion, .51f);
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

            Debug.Log("target name: " + targetRigidbody.name);
            AddExplosionForce(targetRigidbody);

            targetRigidbody.GetComponent<EntityManager>().UpdateHealth(CalculateDamage(targetRigidbody.position));
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

        damage = Mathf.Max(1f, damage);
        Debug.Log("Damage = " + damage);

        return (int)-damage;
    }

    private IEnumerator ChangeColor()
    {
        float duration = Random.Range(1f, 3f);
        while (true)
        {
            float elapsed_time = 0f;
            Color start_c = l.color;
            Color end_c = (l.color.g == Color.red.g) ? Color.red : orange;
            Color tmp = l.color;
            while (elapsed_time < duration)
            {
                tmp = Color.Lerp(start_c, end_c, (elapsed_time / duration));
                l.color = tmp;
                elapsed_time += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator LightFlash()
    {

        float elapsed_time = 0f;
        Color start_c = l.color;
        float start_i = l.intensity;
        Color tmp = l.color;
        float duration = .1f;
        while (elapsed_time < duration)
        {
            tmp = Color.Lerp(start_c, Color.red, (elapsed_time / duration));
            l.color = tmp;
            l.intensity = Mathf.Lerp(start_i, 3f, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        start_c = l.color;
        start_i = 3f;
        duration = .1f;
        while (elapsed_time < duration)
        {
            tmp = Color.Lerp(start_c, Color.black, (elapsed_time / duration));
            l.color = tmp;
            l.intensity = Mathf.Lerp(start_i, 0f, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        l.enabled = false;
    }
}
