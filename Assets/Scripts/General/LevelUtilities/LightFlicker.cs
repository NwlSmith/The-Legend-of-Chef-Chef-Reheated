using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {


    private Light m_light;
    // Use this for initialization
    void Start () {
        m_light = GetComponentInParent<Light>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_light.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_light.enabled = false;
    }


}
