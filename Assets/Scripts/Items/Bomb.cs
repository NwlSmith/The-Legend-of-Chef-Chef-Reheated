using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public LayerMask boss_mask;
    public LayerMask player_mask;
    public LayerMask enemy_mask;
    public LayerMask hurt_entity_mask;

    public float beep_volume = .01f;
    public float boom_volume = 1f;
    public float normal_beep_freq = .9f;
    public float fast_beep_freq = .04f;
    public float m_explosion_radius = 2f;
    public float m_explosion_force = 1000f;
    public float m_max_damage = 6f;

    public bool m_detonating = false;

    public Animator anim;
    public Rigidbody2D rb;
    public GameObject explosion;
    public AudioClip beep;
    public AudioClip[] booms;

    public SpriteRenderer m_green_SR;
    public SpriteRenderer m_red_SR;
    public Light m_light;
    public Light m_beep_light;

    private LayerMask[] target_masks;

    private WaitForSeconds normal_beep_wait;
    private WaitForSeconds fast_beep_wait;
    private WaitForFixedUpdate wait_for_fixed_update = new WaitForFixedUpdate();

    private AudioSource beep_source;
    private AudioSource boom_source;

    private Coroutine normal_beeping_co;
    private Coroutine tracking_co;

    private void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        normal_beep_wait = new WaitForSeconds(normal_beep_freq);
        fast_beep_wait = new WaitForSeconds(fast_beep_freq);

        m_light = transform.GetChild(2).GetComponent<Light>();
        m_beep_light = transform.GetChild(3).GetComponent<Light>();

        beep_source = rb.AddAS();
        beep_source.clip = beep;
        beep_source.volume = beep_volume;
        beep_source.pitch = Random.Range(.9f, 1.1f);
        boom_source = rb.AddAS();
        boom_source.clip = booms[Random.Range(0, booms.Length)];
        boom_source.volume = boom_volume;

        normal_beeping_co = StartCoroutine(NormalBeeping());

        target_masks = new LayerMask[4];
        target_masks[0] = enemy_mask;
        target_masks[1] = player_mask;
        target_masks[2] = boss_mask;
        target_masks[3] = hurt_entity_mask;
	}

    private void Start()
    {
        if (m_green_SR != null && m_green_SR != null)
        {
            tracking_co = StartCoroutine(UpdateProximity());
            StartCoroutine(PlaceBomb());
        }
    }

    private IEnumerator PlaceBomb()
    {
        m_green_SR.transform.localScale = new Vector3(0f, 0f, 1f);
        m_red_SR.transform.localScale = new Vector3(0f, 0f, 1f);
        Vector3 scaleRate = new Vector3(.04f, .04f, 0f);
        WaitForSeconds ratewait = new WaitForSeconds(.01f);
        for (int i = 0; i < 25; i++)
        {
            m_green_SR.transform.localScale += scaleRate;
            m_red_SR.transform.localScale += scaleRate;
            yield return ratewait;
        }
    }

    public void Detonate()
    {
        if (!m_detonating)
            StartCoroutine(DetonationProcess());
    }

    private IEnumerator UpdateProximity()
    {
        while (true)
        {
            float closest = m_explosion_radius;
            for (int i = 0; i < target_masks.Length; i++)
                closest = TrackProximity(target_masks[i], closest);
            UpdateColor(closest);
            yield return wait_for_fixed_update;
        }
    }

    private float TrackProximity(LayerMask lm, float closest)
    {
        // Find all the entities in an area around the bomb and damage them.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_explosion_radius, lm);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody2D targetRigidbody = colliders[i].GetComponent<Rigidbody2D>();
            if (!targetRigidbody)
                continue;


            float dist = (targetRigidbody.position - (Vector2)transform.position).magnitude;

            closest = Mathf.Min(dist, closest);
        }

        return closest;
    }

    private void UpdateColor(float closest)
    {
        if (closest == m_explosion_radius)
            return;
        float fraction;
        fraction = closest / m_explosion_radius;

        Color tmp = m_green_SR.color;
        tmp.a = fraction;
        m_green_SR.color = tmp;

        tmp = m_red_SR.color;
        tmp.a = 1 - fraction;
        m_red_SR.color = tmp;

        tmp = m_light.color;
        tmp.g = fraction;
        tmp.r = 1-fraction;
        m_light.color = tmp;
    }

    private IEnumerator NormalBeeping()
    {
        while (true)
        {
            beep_source.Play();
            yield return normal_beep_wait;
            m_beep_light.enabled = true;
            yield return new WaitForSeconds(.1f);
            m_beep_light.enabled = false;
        }
    }

    public IEnumerator DetonationProcess()
    {
        m_detonating = true;
        if (normal_beeping_co != null)
            StopCoroutine(normal_beeping_co);

        for (int i = 5; i > 0; i--)
        {
            if (m_beep_light.enabled)
                m_beep_light.enabled = false;
            else
                m_beep_light.enabled = true;
            beep_source.Play();
            yield return fast_beep_wait;
        }

        m_beep_light.enabled = false;

        Explode();
        StopCoroutine(tracking_co);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        boom_source.Play();
    }

    private void Explode()
    {
        for (int i = 0; i < target_masks.Length; i++)
            DamageNearby(target_masks[i]);

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        boom_source.Play();

        StartCoroutine(LightFlash());

        explosion.SetActive(true);
        explosion.GetComponent<Animator>().SetTrigger("explode");

        GameManager.instance.CameraShake(7.5f, 1f, true);

        Destroy(gameObject, 1f);
        Destroy(explosion, .5f);
    }

    private IEnumerator LightFlash()
    {
        float elapsed_time = 0f;
        Color start_c = m_beep_light.color;
        float start_i = m_beep_light.intensity;
        float duration = .1f;
        while (elapsed_time < duration)
        {
            m_beep_light.color = Color.Lerp(start_c, Color.green, (elapsed_time / duration));
            m_beep_light.intensity = Mathf.Lerp(start_i, 3f, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }

        start_c = m_beep_light.color;
        start_i = 3f;
        duration = .1f;
        while (elapsed_time < duration)
        {
            m_beep_light.color = Color.Lerp(start_c, Color.black, (elapsed_time / duration));
            m_beep_light.intensity = Mathf.Lerp(start_i, 0f, (elapsed_time / duration));
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        m_beep_light.enabled = false;

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
            if (lm != boss_mask)
                AddExplosionForce(targetRigidbody);

            if (GameManager.instance.debug)
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

        damage = Mathf.Max(1f, damage);

        return (int)-damage;
    }
}
