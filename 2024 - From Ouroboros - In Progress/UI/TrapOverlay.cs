using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapOverlay : MonoBehaviour
{
    public GameObject trapUI;
    public TrapButton up;
    public TrapButton down;

    private void Awake()
    {
        EventModule.i.onTUp += OnTUp;
        EventModule.i.onTDown += OnTDown;
        EventModule.i.onSetUpTrapButtons += SetUpTrapButtons;
        EventModule.i.onSetDownTrapButtons += SetDownTrapButtons;
    }

    public void OnTUp()
    {
        if (trapUI.activeSelf && up.on)
        {
            up.Teleport();
        }
    }

    public void OnTDown()
    {
        if (trapUI.activeSelf && down.on)
        {
            down.Teleport();
        }
    }

    public void SetUpTrapButtons(TrapScript trap)
    {
        trapUI.SetActive(true);
        if (trap.upTrap != null)
            up.SetUp(trap.upTrap);
        if (trap.downTrap != null)
            down.SetUp(trap.downTrap);
    }

    public void SetDownTrapButtons()
    {
        up.SetDown();
        down.SetDown();
        trapUI.SetActive(false);
    }
}
