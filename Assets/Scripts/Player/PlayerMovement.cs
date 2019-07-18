using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private bool m_moving = false;
    private bool m_moving_down = false;
    private bool m_moving_up = false;
    private bool m_moving_right = false;
    private bool m_moving_left = false;

    private float m_move_horizontal;
    private float m_move_vertical;

    private PlayerManager PM;

    void Start()
    {
        PM = GetComponent<PlayerManager>();

        PM.anim.SetTrigger("Move_Down");
    }

    private void Update()
    {
        m_move_horizontal = Input.GetAxis("Horizontal");
        m_move_vertical = Input.GetAxis("Vertical");
    }

    void FixedUpdate ()
    {
        if (!PM.chef_dizzy && PM.can_move)
            HandleInput();
        else
        {
            PM.rb.velocity = Vector2.Lerp(PM.rb.velocity, new Vector2(0f, 0f), .05f);
            PM.walk_source.Stop();
        }
	}

    private void HandleInput()
    {
        Vector2 movement = new Vector2(m_move_horizontal, m_move_vertical);
        PM.rb.velocity = movement * PM.m_speed;
        if (Mathf.Abs(movement.x) > 0 || Mathf.Abs(movement.y) > 0)
        {
            m_moving = true;
            PM.anim.SetBool("Moving", true);
            if (!PM.walk_source.isPlaying)
                PM.walk_source.Play();
        }
        else
        {
            m_moving = false;
            PM.anim.SetBool("Moving", false);
            PM.walk_source.Stop();
        }

        HandleAnimations(PM.rb.velocity.x, PM.rb.velocity.y);
    }

    private void HandleAnimations(float moveX, float moveY)
    {
        if (m_moving)
        {
            if (moveX > 0)
                m_moving_right = true;
            else
                m_moving_right = false;
            PM.anim.SetBool("Moving_Right", m_moving_right);

            if (moveX < 0)
                m_moving_left = true;
            else
                m_moving_left = false;
            PM.anim.SetBool("Moving_Left", m_moving_left);

            if (moveY < 0)
                m_moving_down = true;
            else
                m_moving_down = false;
            PM.anim.SetBool("Moving_Down", m_moving_down);

            if (moveY > 0)
                m_moving_up = true;
            else
                m_moving_up = false;
            PM.anim.SetBool("Moving_Up", m_moving_up);
        }
    }

    public void ResetValues()
    {
        m_moving = false;
        m_moving_down = false;
        m_moving_up = false;
        m_moving_right = false;
        m_moving_left = false;
}
}
