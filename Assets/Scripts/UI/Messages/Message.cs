using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Message : MonoBehaviour {

    [HideInInspector] public int img_num;
    [HideInInspector] public bool m_condition = false;
    [HideInInspector] public bool displaying = false;
    [HideInInspector] public int priority;
    protected Coroutine m_display_co;
    protected Image img;
    protected Color on = new Color(1f, 1f, 1f, 1f);
    protected Color off = new Color(1f, 1f, 1f, 0f);

    protected virtual void Awake()
    {
        img = GetComponent<Image>();
        if (img == null)
            Debug.Log("Message " + img_num + " does not have Image Component");
        img.color = off;
    }

    public void Enable(bool b)
    {
        img.enabled = b;
    }

    public void StartDisplay()
    {
        img.color = on;
        displaying = true;
        if (m_display_co != null)
            StopCoroutine(m_display_co);
        m_display_co = StartCoroutine(Display());
    }

    public void StopDisplay()
    {
        img.color = off;
        displaying = false;
        if (m_display_co != null)
        {
            m_condition = false;
            StopCoroutine(m_display_co);
        }
        Enable(false);
    }

    protected abstract IEnumerator Display();
}
