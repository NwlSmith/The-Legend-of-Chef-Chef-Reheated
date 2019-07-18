using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMessage : Message {

    // flashing, timed message for important notifications, when triggered will play in front of everything else until time is up.

    public float m_duration = 3f;
    private readonly float m_flash_time_float = .25f;
    private WaitForSeconds m_flash_wait_time;

    protected override void Awake()
    {
        priority = 1;
        m_flash_wait_time = new WaitForSeconds(m_flash_time_float);
        base.Awake();
    }

    protected override IEnumerator Display()
    {
        for (int i = 0; i < m_duration * 4; i++)
        {
            img.color = (displaying = !displaying) ? off : on;
            yield return m_flash_wait_time;
        }

        MessageManager mm = GetComponentInParent<MessageManager>();
        if (mm != null)
            mm.StopDisplay(img_num);
        else
            Debug.Log("Error: Tempmessage created with no MessageManager");
    }
}
