using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [HideInInspector] public string itemName = "";
    [HideInInspector] public string targetTag = "Player";
    [HideInInspector] public AudioSource asrc;
    [HideInInspector] public SpriteRenderer sr;

    protected void Initialize()
    {
        transform.localScale = new Vector3(0f, 0f, 1f);
        asrc = GetComponent<AudioSource>();
        if (asrc == null)
            Debug.Log("Item created without AudioSource");
        asrc.volume = .5f;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.Log("Item created without SpriteRenderer");
        StartCoroutine(ScaleItem(new Vector3(1f, 1f, 1f)));
    }

    private IEnumerator ScaleItem(Vector3 end_scale)
    {
        float elapsed_time = 0f;
        float growlifetime = .25f;
        Vector3 start_scale = transform.localScale;

        while (elapsed_time < growlifetime)
        {
            transform.localScale = Vector3.Lerp(start_scale, end_scale, elapsed_time / growlifetime);
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        if (end_scale == Vector3.zero)
            Destroy(gameObject);
    }

    protected void PickUp()
    {
        GetComponent<Collider2D>().enabled = false;
        GameManager.instance.AddItem(itemName);
        asrc.Play();
        StartCoroutine(ScaleItem(Vector3.zero));
    }
}
