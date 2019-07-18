using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawn : MonoBehaviour {

    public Fireball fireball1;
    public Fireball fireball2;
    public bool use_fb1_on_stage2;
    private Fireball fb;

    private void Start()
    {
        fb = fireball2;
    }

    public void Stage2()
    {
        if (fireball1 != null)
            fb = fireball1;
    }

    // fires the currently set fireball. At stage2, that will be fb1
    public void Fire()
    {
        Fireball fbInstance = Instantiate(fb, transform.position, transform.rotation) as Fireball;
        fbInstance.GetComponent<Rigidbody2D>().velocity = fbInstance.m_speed * transform.rotation.eulerAngles;
    }

    //will fire a fb1 if it can
    public void Fire1()
    {
        if (fireball1 == null)
        {
            Debug.Log("Error: this FireBallSpawn is not equipped to fire a Fireball_1");
            return;
        }
        Instantiate(fireball1, transform.position, transform.rotation);
    }
}
