using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModule : MonoBehaviour
{
    public static GravityModule i;

    [NonSerialized]
    public bool gravityUp;

    private Vector3 gravityDirection;
    private float gravitationalConst;
    private Dictionary<string, Vector3> directions;

    private void Awake()
    {
        i = this;

        gravityUp = true;
        gravitationalConst = 9.81f;
        gravityDirection = Physics.gravity.normalized;
    }

    // unused
    private Dictionary<string, Vector3> BuildDirections()
    {
        Dictionary<string, Vector3> d = new Dictionary<string, Vector3>
        {
            { "Up", Vector3.up },
            { "Down", Vector3.down },
            { "Left", Vector3.left },
            { "Right", Vector3.right },
            { "Front", Vector3.forward },
            { "Back", Vector3.back }
        };

        return d;
    }

    // unused
    public void SetGravDir(string dir)
    {
        gravityDirection = directions[dir];
    }

    public void SetGravConst(float grav)
    {
        gravitationalConst = grav;
    }

    public void InvertGravDir()
    {
        gravityDirection *= -1;
        gravityUp = !gravityUp;
        ChangeGravity();
    }

    public void ChangeGravity()
    {
        Physics.gravity = gravityDirection * gravitationalConst;
        EventModule.i.OnGravityChange();
    }
}
