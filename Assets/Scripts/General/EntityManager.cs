using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityManager : MonoBehaviour {

    public float pixel_offset;
    public bool can_attack = true;
    public bool can_move = true;
    public bool can_be_hit = true;
    public bool attacking = false;
    [HideInInspector] public int melee_dmg;
    [HideInInspector] public float melee_force;
    [HideInInspector] public Light m_light;

    [HideInInspector] public Health health;


    public abstract void UpdateHealth(int num);

    public IEnumerator ExtinguishLight()
    {
        WaitForSeconds inc_wait = new WaitForSeconds(.01f);
        while (m_light.range > 0)
        {
            m_light.range = Mathf.Lerp(m_light.range, 0, 1f);
            yield return inc_wait;
        }
    }

    public virtual void PlayerInZone(int zone)
    {
    }
}
