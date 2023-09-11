using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : Collectable
{
    public float cashToGive;

    public override void Collect(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ≤•∑≈“Ù–ß
            AudioManager.instance.PlayAudio(AudioManager.instance.gold);

            ItemAccount.instance.Money(cashToGive);
            Destroy(gameObject);
        }
    }
}
