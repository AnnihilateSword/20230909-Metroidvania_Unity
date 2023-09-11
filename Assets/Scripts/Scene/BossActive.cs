using UnityEngine;

public class BossActive : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BossUI.instance.BossActivator();

            Destroy(gameObject);
            //StartCoroutine(WaitForBoss());
        }
    }

    // µÈ BOSS ³öÀ´
    //IEnumerator WaitForBoss()
    //{
    //    PlayerController.instance.BCanMove = false;
    //    PlayerController.instance.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
    //    PlayerController.instance.GetComponent<Animator>().SetBool("Run", false);
    //    yield return new WaitForSeconds(3.0f);
    //    PlayerController.instance.BCanMove = true;
    //    Destroy(gameObject);
    //}
}
