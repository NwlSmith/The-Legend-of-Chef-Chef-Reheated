using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_2 : Fireball
{
    //big fireball that radiates outward from boss
    public Vector3 start_dir;

/* Rigidbody2D knifeInstance = Instantiate(PM.knives[Random.Range(0, PM.knives.Length)], PM.rb.position + m_knife_throw_offset, m_knife_direction) as Rigidbody2D;
    knifeInstance.velocity = PM.rb.velocity + PM.knife_speed * m_knife_forward; */

    // will need the starting rotation to be set by boss

    protected override void StartMovement()
    {
        rb.velocity = m_speed * transform.up;
    }

    protected override void StopMovement()
    {
        rb.velocity = Vector3.zero;
    }
}
