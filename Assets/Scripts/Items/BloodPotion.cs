using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPotion : MonoBehaviour
{
    public float healthToGive;  // �ظ�����ֵ

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // �ظ�����ֵ
            collision.GetComponentInChildren<PlayerHealth>().Health += healthToGive;
            Destroy(gameObject);
        }
    }
}
