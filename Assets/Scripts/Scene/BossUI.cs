using UnityEngine;
using Cinemachine;

public class BossUI : MonoBehaviour
{
    public GameObject bossPanel;
    public GameObject bossMures;// Boss Χǽ����������
    public GameObject boss;     // Boss
    public CinemachineVirtualCamera cinemachineVirtualCamera;  // �������
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
    /// ���� BOSS ����
    /// </summary>
    public void BossActivator()
    {
        cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = boosCollider2D;

        bossPanel.SetActive(true);
        bossMures.SetActive(true);
        boss.SetActive(true);
    }

    /// <summary>
    /// ���� BOSS ����
    /// </summary>
    public void BossDeactivator()
    {
        bossPanel.SetActive(false);
        bossMures.SetActive(false);

        cinemachineVirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = originalCollider2D;
    }
}
