using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public int m_health;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateHealth(int num)
    {
        m_health += num;
    }

    public void SetHealth(int num)
    {
        m_health = num;
    }

    public int GetHealth()
    {
        return m_health;
    }
}
