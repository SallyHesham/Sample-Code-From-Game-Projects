using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventModule : MonoBehaviour
{
    public static EventModule i;

    private void Awake()
    {
        i = this;
    }

    public event Action onGravityChange;
    public event Action onTUp;
    public event Action onTDown;
    public event Action<TrapScript> onSetUpTrapButtons;
    public event Action onSetDownTrapButtons;

    public void OnGravityChange()
    {
        onGravityChange?.Invoke();
    }

    public void OnTUp()
    {
        onTUp?.Invoke();
    }

    public void OnTDown()
    {
        onTDown?.Invoke();
    }

    public void OnSetUpTrapButtons(TrapScript t)
    {
        onSetUpTrapButtons?.Invoke(t);
    }

    public void OnSetDownTrapButtons()
    {
        onSetDownTrapButtons?.Invoke();
    }

    void debuglog()
    {
        Debug.Log("Invoked");
    }
}
