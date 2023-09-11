using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public string objectName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect(collision);
        }
    }

    public abstract void Collect(Collider2D collision);
}
