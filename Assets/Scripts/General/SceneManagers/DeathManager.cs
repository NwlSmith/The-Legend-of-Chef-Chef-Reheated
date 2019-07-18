using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathManager : StoryManager
{
    private Text respawn;
    private Color on = new Color(1f, 1f, 1f, 1f);
    private Color off = new Color(1f, 1f, 1f, 0f);
    private readonly float m_flash_time_float = .5f;
    private WaitForSeconds m_flash_wait_time;

    protected override void Awake()
    {
        respawn = GetComponentInChildren<Text>();
        m_flash_wait_time = new WaitForSeconds(m_flash_time_float);
        nextSceneString = "Level" + GameManager.instance.cur_level.ToString();
        Debug.Log("nextSceneString = " + nextSceneString);
        base.Awake();
        StartCoroutine(PressEnterToRespawn());
    }


    private IEnumerator PressEnterToRespawn()
    {
        yield return new WaitUntil(() => black.color.a == 0);
        bool enabled = false;
        while (true)
        {
            respawn.color = (enabled = !enabled) ? off : on;
            yield return m_flash_wait_time;
        }
    }
}