using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Block")
        {
            gameObject.GetComponent<SpriteRenderer>().material =
                collision.gameObject.GetComponent<SpriteRenderer>().material;
            gameObject.GetComponent<ColorState>().state =
                collision.gameObject.GetComponent<ColorState>().state;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Block")
        {
            gameObject.GetComponent<SpriteRenderer>().material = GameAssets.Instance.snakeGlow;
            gameObject.GetComponent<ColorState>().state = -1;
        }
    }
}
