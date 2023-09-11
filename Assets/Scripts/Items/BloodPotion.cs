using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPotion : MonoBehaviour
{
    public float healthToGive;  // 回复生命值

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 回复生命值
            collision.GetComponentInChildren<PlayerHealth>().Health += healthToGive;
            Destroy(gameObject);
        }
    }
}
