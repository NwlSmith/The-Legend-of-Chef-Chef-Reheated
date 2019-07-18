using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelect : MonoBehaviour {

    private CursorManager cm;

    private void Start()
    {
        cm = GetComponentInParent<MenuManager>().cm;
    }

    public void MouseOver()
    {
        Debug.Log("Mouseover");
        //cm.transform.position = transform.position;
        cm.UpdatePos(transform.position);
    }

}
