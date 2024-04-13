using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    bool rotating = false;
    Vector3 originalPos;
    float rateOfRot = 1.5f;
    Transform playerTran;
    Vector3 target;

    public bool openToTheRedArrow = true;

    private void Start()
    {
        originalPos = transform.forward;
        playerTran = GameObject.Find("Player").transform;
        // defines dir and mag of opening the door
        if (openToTheRedArrow) target = Vector3.up * 90;
        else target = Vector3.down * 90;
    }

    void Interact()
    {
        if (!rotating)
        {
            // close door
            if (transform.forward != originalPos)
            {
                StartCoroutine(RotateClose());
            }
            // open door
            else
            {
                StartCoroutine(RotateOpen());
            }
            
        }
    }

    private IEnumerator RotateOpen()
    {
        rotating = true;
        Quaternion to = transform.rotation * Quaternion.Euler(target);
        Quaternion from = transform.rotation;
        float timeCount = 0f;
        float rate = rateOfRot;

        while (transform.rotation != to)
        {
            transform.rotation = Quaternion.Slerp(from, to, timeCount);
            timeCount += Time.fixedDeltaTime * rate;

            yield return new WaitForFixedUpdate();
        }
        rotating = false;
    }

    private IEnumerator RotateClose()
    {
        rotating = true;
        Quaternion to = transform.rotation * Quaternion.Euler(-target);
        Quaternion from = transform.rotation;
        float timeCount = 0f;
        float rate = rateOfRot;

        while (transform.rotation != to)
        {
            transform.rotation = Quaternion.Slerp(from, to, timeCount);
            timeCount += Time.fixedDeltaTime * rate;

            yield return new WaitForFixedUpdate();
        }
        rotating = false;
    }
}
