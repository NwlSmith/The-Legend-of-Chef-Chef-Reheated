using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleProjectile : MonoBehaviour {

    public bool hit;
    public float speed = 5f;
    public int dmg = 2;
    public float push_force = 500f;

    public AudioClip travel_sound;
    public AudioClip hit_sound;
    private AudioSource sound_source;

    private Coroutine hit_co;

    private Rigidbody2D rb;

	private void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();

        sound_source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        sound_source.Play();
        StartCoroutine(SoundChange());
        
        rb.velocity = speed * -transform.up;

        Invoke("Timeout", 5f);
    }

    private IEnumerator SoundChange()
    {
        yield return new WaitForSeconds(2f);
        sound_source.clip = travel_sound;
        sound_source.loop = true;
        sound_source.volume = .1f;
        sound_source.Play();
        yield return new WaitUntil(() => hit);
        sound_source.clip = hit_sound;
        sound_source.loop = false;
        sound_source.volume = 1f;
        sound_source.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            hit = true;
            if (hit_co == null)
                hit_co = StartCoroutine(HitObject());
            PlayerManager pm = PlayerManager.instance;
            Vector2 hit_dir = pm.rb.position - (Vector2)transform.position;
            pm.rb.AddForce(hit_dir * push_force);
            Debug.Log("Hitting player!");
            GetComponent<Collider2D>().enabled = false;

            pm.UpdateHealth(-dmg);
        }
    }

    private void Timeout()
    {
        hit = true;
        if (hit_co == null)
            hit_co = StartCoroutine(HitObject());
    }

    private IEnumerator HitObject()
    {
        Light l = GetComponentInChildren<Light>();
        float duration = .1f;
        float elapsed_time = 0f;
        float start_intensity = l.intensity;
        float end_intensity = 10f;
        float start_range = l.range;
        float end_range = 50f;
        while (elapsed_time < duration)
        {
            float timestep = elapsed_time / duration;
            l.intensity = Mathf.Lerp(start_intensity, end_intensity, timestep);
            l.range = Mathf.Lerp(start_range, end_range, timestep);
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        duration = .8f;
        elapsed_time = 0f;
        start_intensity = l.intensity;
        start_range = l.range;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float start_alpha = sr.color.a;
        while (elapsed_time < duration)
        {
            float timestep = elapsed_time / duration;
            l.intensity = Mathf.Lerp(start_intensity, 0f, timestep);
            l.range = Mathf.Lerp(start_range, 0f, timestep);
            sr.color = new Color (1f, 1f, 1f, Mathf.Lerp(start_alpha, 0f, timestep));
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject, 0f);
    }
}
