using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    public bool locked = true;
    public string not_fulfill_WC;
    public string leave;

    public MessageManager mm;
    private GameManager gm;

    private Color green = new Color(0f, 1f, 0f, 1f);
    private Color red = new Color(1f, 0f, 0f, 1f);
    private Color white = new Color(1f, 1f, 1f, 1f);
    private float c_speed = 2f;
    private WaitForSeconds timestep = new WaitForSeconds(.05f);
    private Light m_light;

    private void Start()
    {
        gm = GameManager.instance;
        m_light = GetComponentInChildren<Light>();
        StartCoroutine(FlashingLight());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Determine what message needs to be displayed based on the progress toward the wincondition
        if (collision.tag == "Player")
        {
            if (locked)
                gm.StartDisplayMessage(not_fulfill_WC);
            else
                gm.StartDisplayMessage(leave);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Input.GetButtonDown("Enter") && !locked)
        {
            gm.levelManager.LevelComplete();
            
            StopAllCoroutines();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Determine what message needs to be displayed based on the progress toward the wincondition
        if (collision.tag == "Player")
        {
            if (locked)
                gm.StopDisplayMessage(not_fulfill_WC);
            else
                gm.StopDisplayMessage(leave);
        }
    }

    private IEnumerator FlashingLight()
    {
        Color cur = m_light.color;
        Color target = white;
        while(true)
        {
            float timer = 0f;
            if (target == white)
                target = locked ? red : green;
            else
                target = white;
            while (!IsColor(target))
            {
                timer += Time.deltaTime;
                cur.r = Mathf.Lerp(m_light.color.r, target.r, timer * c_speed);
                cur.g = Mathf.Lerp(m_light.color.g, target.g, timer * c_speed);
                cur.b = Mathf.Lerp(m_light.color.b, target.b, timer * c_speed);
                m_light.color = cur;
                yield return timestep;
            }
        }
    }

    private bool IsColor(Color other)
    {
        if ((int)(m_light.color.r * 100) != (int)(other.r * 100))
            return false;
        if ((int)(m_light.color.g * 100) != (int)(other.g * 100))
            return false;
        if ((int)(m_light.color.b * 100) != (int)(other.b * 100))
            return false;
        return true;
    }
}
