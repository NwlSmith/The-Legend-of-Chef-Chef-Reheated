using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_1 : Fireball
{
    //smaller fireball that follows player


    private Transform target;
    private bool moving = true;
    private float follow_speed = .01f;

    protected override void Start()
    {
        //rb.velocity = m_speed * Vector2.up;
        target = PlayerManager.instance.transform;
        
        base.Start();
    }

    protected override void StartMovement()
    {
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            follow_speed = m_speed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, follow_speed);
            Vector3 targ = target.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
        }
    }

    protected override void StopMovement()
    {
        rb.velocity = Vector3.zero;
        moving = false;
    }
}
