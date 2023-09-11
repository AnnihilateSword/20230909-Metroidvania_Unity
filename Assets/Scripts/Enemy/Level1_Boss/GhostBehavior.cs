using UnityEngine;

public class GhostBehavior : MonoBehaviour
{
    public Transform[] transforms;
    public GameObject flame;    // ��������

    public float timeToShoot, countDown;
    public float timeToTP, countDownTP;
    private void OnEnable()
    {
        //Random.InitState((int)Time.time);  // ��ʼ�����������������
        //int initialPosition = Random.Range(0, transforms.Length);
        transform.position = transforms[3].position;
        countDown = timeToShoot;
        countDownTP = timeToTP;

        // ת��
        BossFlip();
    }

    private void Update()
    {
        CountDowns();
    }

    /// <summary>
    /// BOSS ����
    /// </summary>
    private void OnDestroy()
    {
        // BOSS �����ܣ�����Χ������� BOSS ����
        BossUI.instance.BossDeactivator();

        // ��Ϸʤ����
        GameManager.instance.GameSuccess();
    }

    public void CountDowns()
    {
        countDown -= Time.deltaTime;
        countDownTP -= Time.deltaTime;

        if (countDown < 0.0f)
        {
            countDown = timeToShoot;
            ShootPlayer();
        }

        if (countDownTP < 0.0f)
        {
            countDownTP = timeToTP;
            Teleport();
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public void ShootPlayer()
    {
        // ������Ч
        AudioManager.instance.PlayAudio(AudioManager.instance.flame);

        GameObject spell = Instantiate(flame, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Teleport()
    {
        Random.InitState((int)Time.time);  // ��ʼ�����������������
        int initialPosition = Random.Range(0, transforms.Length);
        transform.position = transforms[initialPosition].position;

        // ת��
        BossFlip();
    }

    /// <summary>
    /// ת��
    /// </summary>
    public void BossFlip()
    {
        if (transform.position.x > PlayerController.instance.transform.position.x)
            transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
        else if (transform.position.x < PlayerController.instance.transform.position.x)
            transform.localScale = new Vector3(-2.0f, 2.0f, 1.0f);
    }
}
