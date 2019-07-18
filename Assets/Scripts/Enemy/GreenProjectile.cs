using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenProjectile : MonoBehaviour
{

    public float m_explosion_radius = 2f;
    public float m_explosion_force = 1000f;
    public float m_max_damage = 2f;

    private readonly WaitForSeconds normal_beep_wait = new WaitForSeconds(.9f);
    private readonly float m_speed = 4f;

    public LayerMask player_mask;
    public LayerMask enemy_mask;
    public LayerMask hurt_entity_mask;
    private LayerMask[] target_masks;

    public AudioClip throw_sound;
    public AudioClip squeek;
    public AudioClip[] booms;
    private AudioSource throw_source;
    private AudioSource squeek_source;
    private AudioSource boom_source;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private GameObject explosion;
    private Light l;
    private Color green = new Color(0f, 1f, 0f, 1f);
    private Color red = new Color(1f, 0f, 0f, 1f);

    public GameObject bomb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        explosion = transform.GetChild(0).gameObject;
        l = transform.GetChild(1).GetComponent<Light>();

        throw_source = rb.AddAS();
        throw_source.clip = throw_sound;
        throw_source.pitch = .5f;
        throw_source.volume = 2f;
        squeek_source = rb.AddAS();
        squeek_source.clip = squeek;
        boom_source = rb.AddAS();
        boom_source.clip = booms[Random.Range(0, booms.Length)];

        target_masks = new LayerMask[3];
        target_masks[0] = player_mask;
        target_masks[1] = enemy_mask;
        target_masks[2] = hurt_entity_mask;
    }
    

    private IEnumerator NormalFlashing()
    {

        while (true)
        {
            yield return normal_beep_wait;
            l.color = red;
            yield return new WaitForSeconds(.1f);
            l.color = green;
        }
    }

    private void Start()
    {
        StartCoroutine(PlaceItem());
        rb.velocity = m_speed * -transform.up;
        rb.angularVelocity = Random.Range(-5, 5) * 50f;
        throw_source.Play();
    }

    private IEnumerator PlaceItem()
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "BossProjectile")
        { return; }
        //have the bomb detonate

        rb.velocity = 0f * transform.up;
        rb.angularVelocity = 0f;

        if (other.tag == "Player" && other.GetComponent<PlayerManager>().attacking)
            StartCoroutine(Disappear());
        else
            Explode();

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
    }

    private void Explode()
    {
        for (int i = 0; i < target_masks.Length; i++)
            DamageNearby(target_masks[i]);

        boom_source.Play();

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
            AddExplosionForce(targetRigidbody);

            Debug.Log("target name: " + targetRigidbody.name);

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

        damage = Mathf.Max(0f, damage);

        return (int)-damage;
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
            l.color = Color.Lerp(start_c, Color.green, (elapsed_time / duration));
            l.intensity = Mathf.Lerp(start_i, 3f, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        start_c = l.color;
        start_i = 3f;
        duration = .1f;
        while (elapsed_time < duration)
        {
            l.color = Color.Lerp(start_c, Color.black, (elapsed_time / duration));
            l.intensity = Mathf.Lerp(start_i, 0f, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        l.enabled = false;
    }

    private IEnumerator Disappear()
    {
        //Vector3 scaleRate = new Vector3(.01f, .01f, 0f);
        //Vector3 posRate = new Vector3(0f, .005f, 0f);
        //WaitForSeconds ratewait = new WaitForSeconds(.01f);
        rb.angularVelocity = 400f;
        //for (int i = 0; i < 50; i++)
        //{
        //    if (transform.localScale.x >= scaleRate.x && transform.localScale.y >= scaleRate.y)
        //    {
        //        transform.localScale -= scaleRate;
        //        transform.localPosition -= posRate;
        //    }
        //    yield return ratewait;
        //}


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

        transform.localScale = new Vector3(.25f, .25f, .25f);

        sr.enabled = false;
        explosion.SetActive(true);
        squeek_source.Play();
        explosion.GetComponent<Animator>().SetTrigger("explode");
        Instantiate(bomb, transform.position, Quaternion.identity);
    }
}
