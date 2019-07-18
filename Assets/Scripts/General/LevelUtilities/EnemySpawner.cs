using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public int max_spawns = 6;
    public float delay_float = 4f;
    private WaitForSeconds delay;
    private List<EntityManager> spawned_enemies = new List<EntityManager>();

    [HideInInspector] public Light m_light;

    private LevelManager lm;
    private Coroutine spawn_co;
    private AudioSource close_sound;

    private void Awake()
    {
        m_light = GetComponentInChildren<Light>();
        delay = new WaitForSeconds(delay_float);

        close_sound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        lm = LevelManager.instance;

        spawn_co = StartCoroutine(SpawnEnemies());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && GetComponent<Collider2D>().enabled)
            GameManager.instance.StartDisplayMessage("press_close");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && GetComponent<Collider2D>().enabled)
            GameManager.instance.StopDisplayMessage("press_close");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Input.GetButtonDown("Enter"))
        {
            GameManager.instance.StopDisplayMessage("press_close");
            GameManager.instance.CameraShake(3f, .1f, true);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Animator>().SetTrigger("Open");
            StopCoroutine(spawn_co);
            StartCoroutine(ExtinguishLight());
            lm.ClosedPipe();
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            RemoveDeadSpawns();
            if (spawned_enemies.Count < 6)
            {
                Vector3 spawn_point = GenerateSpawnpoint();
                spawned_enemies.Add(lm.SpawnRat(spawn_point));
            }
            yield return delay;
        }
    }

    private void RemoveDeadSpawns()
    {
        List<EntityManager> removing = new List<EntityManager>();
        foreach (EntityManager em in spawned_enemies)
        {
            if (em == null)
                removing.Add(em);
        }

        foreach (EntityManager em in removing)
        {
            spawned_enemies.Remove(em);
        }

        //OR

        spawned_enemies.RemoveAll(enemy => enemy == null);
    }

    private Vector3 GenerateSpawnpoint()
    {
        Vector3 spawn_point = new Vector3(transform.position.x + Random.Range(-1, 2), transform.position.y + Random.Range(-1, 2), 0f);
        while (spawn_point == transform.position)
        {
            spawn_point = new Vector3(transform.position.x + Random.Range(-1, 2), transform.position.y + Random.Range(-1, 2), 0f);
        }
        return spawn_point;
    }

    private IEnumerator ExtinguishLight()
    {
        WaitForSeconds inc_wait = new WaitForSeconds(.05f);
        while (m_light.range > 0)
        {
            m_light.range = Mathf.Lerp(m_light.range, 0, 1f);
            yield return inc_wait;
        }
        Destroy(m_light.gameObject);
        close_sound.Play();
    }
}
