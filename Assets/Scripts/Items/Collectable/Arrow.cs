using UnityEngine;

public class Arrow : Collectable
{
    public uint arrowToGive = 30;

    public override void Collect(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ItemAccount.instance.Arraw(arrowToGive);
            Destroy(gameObject);
        }
    }
}
