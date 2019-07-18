using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

    private bool m_is_regen = false;
    private Vector2 m_knife_throw_offset = new Vector2(0f, -.25f);
    private Coroutine m_regen_co;
    private PlayerManager PM;

    private float m_knife_horizontal;
    private float m_knife_vertical;
    private Vector2 m_knife_forward;
    private Quaternion m_knife_direction;

    void Start () {
        PM = GetComponent<PlayerManager>();

        if (PM.gm.inventory.Contains("Graters"))
        {
            PM.melee_dmg = 2;
            PM.anim.SetBool("Graters", true);
        }
        else if (PM.gm.inventory.Contains("Ladles"))
            PM.anim.SetBool("Ladles", true);
        ResetKnifeVariables();
    }

    private void ResetKnifeVariables()
    {
        m_knife_forward = transform.up;
        m_knife_direction = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update ()
    {
        // if the game is paused, don't take input.
        if (PM.gm.paused)
            return;

        if (Input.GetButtonDown("Bomb Set") && HasBomb() && !PM.attacking && !PM.chef_throwing && PM.can_attack)
            StartCoroutine(PlantBomb());
        if (Input.GetButtonDown("Bomb Detonate") && BombsPlaced())
            StartCoroutine(DetonateBombs());

        if (PM.gm.inventory.Contains("KnifeSet"))
        {
            m_knife_horizontal = Input.GetAxis("Knife Horizontal");
            m_knife_vertical = Input.GetAxis("Knife Vertical");
            if ((m_knife_horizontal != 0 || m_knife_vertical != 0) && !PM.chef_throwing && !PM.chef_dizzy)
            {
                if (PM.chef_energy > 0)
                    StartCoroutine(ThrowKnife());
                else
                    StartCoroutine(PM.ChefDizzy());
            }
        }

        if (Input.GetButton("Melee"))
        {
            if (PM.can_attack && !PM.chef_dizzy)
            {
                if (PM.chef_energy <= 0)
                {
                    StartCoroutine(PM.ChefDizzy());
                    PM.gm.StartDisplayMessage("no_energy");
                }
                else if (!PM.gm.inventory.Contains("Ladles") && !PM.gm.inventory.Contains("Graters"))
                    StartCoroutine(PM.ChefDizzy());
                else
                    StartCoroutine(MeleeAttack());
                    
            }
        }
        else if (!m_is_regen && PM.chef_energy < 8)
            m_regen_co = StartCoroutine(RegenEnergy());

    }

    private IEnumerator MeleeAttack()
    {
        PM.attacking = true;
        PM.anim.SetTrigger("Attack_Melee");
        PM.can_attack = false;
        PM.melee_attack_source.clip = PM.melee_attack_sounds[Random.Range(0, PM.melee_attack_sounds.Length)];
        PM.melee_attack_source.Play();
        PM.UpdateEnergy(-1);
        PM.m_speed = PM.m_dash_speed;
        yield return PM.melee_time;
        PM.attacking = false;
        PM.can_attack = true;
        if (m_is_regen && m_regen_co != null)
        {
            StopCoroutine(m_regen_co);
            m_is_regen = false;
        }
        PM.m_speed = PM.m_normal_speed;
        FixPostMelee();
    }

    public void PlayHitSound()
    {
        if (PM.gm.inventory.Contains("Graters"))
            PM.hit_source.clip = PM.grater_hit_sound;
        else
            PM.hit_source.clip = PM.ladle_hit_sounds[Random.Range(0, PM.ladle_hit_sounds.Length)];
        PM.hit_source.pitch = Random.Range(.95f, 1.05f);
        PM.hit_source.Play();
    }


    public IEnumerator PlantBomb()
    {
        PM.can_move = false;
        PM.can_attack = false;
        PM.anim.SetTrigger("Place_Bomb");

        yield return PM.melee_time;

        PM.can_move = true;
        PM.can_attack = true;
        GameObject bombInstance = Instantiate(PM.bomb, PM.rb.position + m_knife_throw_offset, Quaternion.identity) as GameObject;
        PM.bombs.Enqueue(bombInstance);

        PM.gm.RemoveItem("Bomb");
    }

    // Need to implement inventory
    public bool HasBomb()
    {
        return PM.gm.inventory.Contains("Bomb");
    }

    // Need to keep track of bomb objects
    public bool BombsPlaced()
    {
        return PM.bombs.Count > 0;
    }

    private IEnumerator DetonateBombs()
    {
        while (BombsPlaced())
        {
            // detonate bomb
            GameObject bomb = PM.bombs.Dequeue();
            bomb.GetComponent<Bomb>().Detonate();
            yield return PM.quarter_second;
        }
    }

    private IEnumerator ThrowKnife()
    {
        PM.chef_throwing = true;
        PM.anim.SetTrigger("Attack_Knife");
        PM.knife_throw_source.clip = PM.knife_throw_sounds[Random.Range(0, PM.knife_throw_sounds.Length)];
        PM.knife_throw_source.Play();
        PM.UpdateEnergy(-1);
        CalcThrowDirection();
        Rigidbody2D knifeInstance = Instantiate(PM.knives[Random.Range(0, PM.knives.Length)], PM.rb.position + m_knife_throw_offset, m_knife_direction) as Rigidbody2D;
        knifeInstance.velocity = PM.rb.velocity + PM.knife_speed * m_knife_forward;

        yield return new WaitForSeconds(.313f);

        PM.chef_throwing = false;
        if (m_is_regen && m_regen_co != null)
        {
            StopCoroutine(m_regen_co);
            m_is_regen = false;
        }
        FixPostMelee();
    }

    private void CalcThrowDirection()
    {
        ResetKnifeVariables();
        if (Mathf.Approximately(m_knife_horizontal, 0))
        {
            if (m_knife_vertical > 0)
            {
                return;
            }
            else if (m_knife_vertical < 0)
            {
                m_knife_forward = -transform.up;
                m_knife_direction = Quaternion.Euler(0f, 0f, 180f);
            }
        }
        else if (Mathf.Approximately(m_knife_vertical, 0))
        {
            if (m_knife_horizontal > 0)
            {
                m_knife_forward = transform.right;
                m_knife_direction = Quaternion.Euler(0f, 0f, -90f);
            }
            else if (m_knife_horizontal < 0)
            {
                m_knife_forward = -transform.right;
                m_knife_direction = Quaternion.Euler(0f, 0f, 90f);
            }
        }
    }

    private void FixPostMelee()
    {
        if (!PM.anim.GetBool("Moving_Down") && !PM.anim.GetBool("Moving_Up") && !PM.anim.GetBool("Moving_Left") && !PM.anim.GetBool("Moving_Right"))
            PM.anim.SetTrigger("Move_Down");
    }

    private IEnumerator RegenEnergy()
    {
        m_is_regen = true;
        
        yield return PM.energy_regen_time;
        PM.UpdateEnergy(1);
        
        m_is_regen = false;
    }
}
