using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    bool moving = false;
    Vector3 originalPos;
    float rateOfMov = 1.5f;
    Vector3 target;

    private void Start()
    {
        originalPos = transform.position;
        // defines dir and mag of opening the door
        target = transform.position + new Vector3(0, 2, 0);
    }

    void Interact()
    {
        if (!moving)
        {
            // close door
            if (transform.position != originalPos)
            {
                StartCoroutine(SlideClose());
            }
            // open door
            else
            {
                StartCoroutine(SlideOpen());
            }

        }
    }

    private IEnumerator SlideOpen()
    {
        moving = true;
        Vector3 to = target;
        Vector3 from = transform.position;
        float timeCount = 0f;
        float rate = rateOfMov;

        while (transform.position != to)
        {
            transform.position = Vector3.Lerp(from, to, timeCount);
            timeCount += Time.fixedDeltaTime * rate;

            yield return new WaitForFixedUpdate();
        }
        moving = false;
    }

    private IEnumerator SlideClose()
    {
        moving = true;
        Vector3 to = originalPos;
        Vector3 from = transform.position;
        float timeCount = 0f;
        float rate = rateOfMov;

        while (transform.position != to)
        {
            transform.position = Vector3.Lerp(from, to, timeCount);
            timeCount += Time.fixedDeltaTime * rate;

            yield return new WaitForFixedUpdate();
        }
        moving = false;
    }
}
