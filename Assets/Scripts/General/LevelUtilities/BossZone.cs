using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZone : MonoBehaviour {

    public int zone = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            EntityManager bm = FindObjectOfType<Boss1Manager>();
            if (bm == null)
                bm = FindObjectOfType<Boss2Manager>();
            if (bm != null)
                bm.PlayerInZone(zone);
        }
    }
}
