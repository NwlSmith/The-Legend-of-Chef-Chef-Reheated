using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRange : MonoBehaviour {

    private Boss1Manager bm;

	// Use this for initialization
	void Start () {
        bm = transform.GetComponentInParent<Boss1Manager>();
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            bm.in_range = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
            bm.in_range = false;
    }
}
