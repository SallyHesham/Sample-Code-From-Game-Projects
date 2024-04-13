using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapButton : MonoBehaviour
{
    GameObject trap;
    Button button;
    GameObject player;

    [NonSerialized]
    public bool on;

    private void Awake()
    {
        button = GetComponent<Button>();
        on = false;
    }

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    public void SetUp(GameObject t)
    {
        if (t != null)
        {
            trap = t;
            button.interactable = true;
            on = true;
        }
    }

    public void SetDown()
    {
        on = false;
        trap = null;
        button.interactable = false;
    }

    public void Teleport()
    {
        Vector3 target;
        if (GravityModule.i.gravityUp)
            target = trap.transform.position + new Vector3 (0, 1, 0);
        else
            target = trap.transform.position + new Vector3 (0, -1, 0);

        player.transform.position = target;
    }
}
