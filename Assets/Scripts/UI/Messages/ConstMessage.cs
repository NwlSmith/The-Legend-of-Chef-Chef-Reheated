using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstMessage : Message {

    // flashing, constant message for low priority notifications

    private readonly float m_flash_time_float = .25f;
    private WaitForSeconds m_flash_wait_time;

    protected override void Awake()
    {
        priority = 3;
        m_flash_wait_time = new WaitForSeconds(m_flash_time_float);
        base.Awake();
    }

    protected override IEnumerator Display()
    {
        while (true)
        {
            // the color is, if display (which equals its opposite now) is true, off, if it is false, on
            // if there is a problem, switch on and off.
            img.color = (displaying = !displaying) ? off : on;
            yield return m_flash_wait_time;
        }
    }
}
