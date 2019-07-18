using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantryContainer : MonoBehaviour {

    public Item[] possible_items;
    public Transform spawnpoint;
    public float delay_float = .5f;
    private WaitForSeconds delay;

    [HideInInspector] public Light m_light;

    private AudioSource close_source;

    private void Awake()
    {
        m_light = GetComponentInChildren<Light>();
        delay = new WaitForSeconds(delay_float);

        close_source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && GetComponent<Collider2D>().enabled)
            GameManager.instance.StartDisplayMessage("press_open");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && GetComponent<Collider2D>().enabled)
            GameManager.instance.StopDisplayMessage("press_open");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Input.GetButtonDown("Enter"))
        {
            GameManager.instance.StopDisplayMessage("press_open");
            GameManager.instance.CameraShake(3f, .1f, true);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Animator>().SetTrigger("Open");
            close_source.Play();
            StartCoroutine(SpawnAfterDelay());
        }
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return delay;
        Instantiate(possible_items[Random.Range(0, possible_items.Length)], spawnpoint.position, Quaternion.identity);
        StartCoroutine(ExtinguishLight());
    }

    private IEnumerator ExtinguishLight()
    {
        WaitForSeconds inc_wait = new WaitForSeconds(.05f);
        while (m_light.range > 0)
        {
            m_light.range = Mathf.Lerp(m_light.range, 0, 1f);
            yield return inc_wait;
        }
    }
}
