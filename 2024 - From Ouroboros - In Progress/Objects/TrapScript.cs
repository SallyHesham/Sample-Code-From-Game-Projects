using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    public GameObject upTrap;
    public GameObject downTrap;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            EventModule.i.OnSetUpTrapButtons(this);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            EventModule.i.OnSetDownTrapButtons();
        }
    }
}
