using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour {

    public int m_dmg = 1;
    public float m_speed = 15;
    public float m_time;
    public float m_lifetime = 3f;
    public float m_push_force = 300f;
    public float m_hit_volume = .5f;
    public bool m_hit = false;

    private AudioSource m_hit_source;
    private Collider2D m_collider2D;

    private void Awake()
    {
        m_hit_source = GetComponent<AudioSource>();
        m_hit_source.pitch = Random.Range(.9f, 1.1f);
        m_hit_source.volume = m_hit_volume;
        m_time = 0f;
        m_collider2D = GetComponent<Collider2D>();
        m_collider2D.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(EnableQuick());
    }

    private IEnumerator EnableQuick()
    {
        yield return new WaitForSeconds(.045f);
        m_collider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            m_hit = true;
            m_hit_source.Play();
            if (other.GetComponent<Rat3Manager>() == null)
            {
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                Vector2 hit_dir = rb.position - (Vector2)transform.position;
                rb.AddForce(hit_dir * m_push_force);
                if (other.GetComponent<EnemyManager>() != null)
                    other.GetComponent<EnemyManager>().UpdateHealth(-m_dmg);
                else if (other.GetComponent<Boss2Manager>() != null && other.GetComponent<Boss2Manager>().can_be_hit == true)
                {
                    other.GetComponent<Boss2Manager>().UpdateHealth(-m_dmg);
                    other.GetComponent<Boss2Manager>().Retaliate();
                }
            }
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            m_collider2D.enabled = false;
        }
        //Put condition for boundary
    }

    private IEnumerator DestroyMe()
    {
        while (!m_hit && m_time < m_lifetime)
        {
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }


}
