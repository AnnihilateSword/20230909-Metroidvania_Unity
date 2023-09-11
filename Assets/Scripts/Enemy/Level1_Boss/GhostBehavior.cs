using UnityEngine;

public class GhostBehavior : MonoBehaviour
{
    public Transform[] transforms;
    public GameObject flame;    // 攻击火焰

    public float timeToShoot, countDown;
    public float timeToTP, countDownTP;
    private void OnEnable()
    {
        //Random.InitState((int)Time.time);  // 初始化随机数生成器种子
        //int initialPosition = Random.Range(0, transforms.Length);
        transform.position = transforms[3].position;
        countDown = timeToShoot;
        countDownTP = timeToTP;

        // 转向
        BossFlip();
    }

    private void Update()
    {
        CountDowns();
    }

    /// <summary>
    /// BOSS 死亡
    /// </summary>
    private void OnDestroy()
    {
        // BOSS 被击败，禁用围栏，解除 BOSS 房间
        BossUI.instance.BossDeactivator();

        // 游戏胜利！
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
    /// 开火
    /// </summary>
    public void ShootPlayer()
    {
        // 播放音效
        AudioManager.instance.PlayAudio(AudioManager.instance.flame);

        GameObject spell = Instantiate(flame, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// 传送
    /// </summary>
    public void Teleport()
    {
        Random.InitState((int)Time.time);  // 初始化随机数生成器种子
        int initialPosition = Random.Range(0, transforms.Length);
        transform.position = transforms[initialPosition].position;

        // 转向
        BossFlip();
    }

    /// <summary>
    /// 转向
    /// </summary>
    public void BossFlip()
    {
        if (transform.position.x > PlayerController.instance.transform.position.x)
            transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
        else if (transform.position.x < PlayerController.instance.transform.position.x)
            transform.localScale = new Vector3(-2.0f, 2.0f, 1.0f);
    }
}
