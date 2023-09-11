using UnityEngine;
using Cinemachine;

public class BossUI : MonoBehaviour
{
    public GameObject bossPanel;
    public GameObject bossMures;// Boss 围墙，不能逃跑
    public GameObject boss;     // Boss
    public CinemachineVirtualCamera cinemachineVirtualCamera;  // 虚拟相机
    public Collider2D boosCollider2D;
    public Collider2D originalCollider2D;

    public static BossUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        bossPanel.SetActive(false);
        bossMures.SetActive(false);
        boss.SetActive(false);
    }

    /// <summary>
    /// 激活 BOSS 房间
    /// </summary>
    public void BossActivator()
    {
        cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = boosCollider2D;

        bossPanel.SetActive(true);
        bossMures.SetActive(true);
        boss.SetActive(true);
    }

    /// <summary>
    /// 禁用 BOSS 房间
    /// </summary>
    public void BossDeactivator()
    {
        bossPanel.SetActive(false);
        bossMures.SetActive(false);

        cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = originalCollider2D;
    }
}
