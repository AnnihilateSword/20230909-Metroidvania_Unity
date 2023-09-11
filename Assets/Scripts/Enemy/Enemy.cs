using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;                            // ��������
    public float maxHealthPoint;                        // �������ֵ
    [SerializeField] protected float currentHealthPoint;  // ��ǰ����ֵ
    public float CurrentHealthPoint
    {
        get { return currentHealthPoint; }
        set { currentHealthPoint = value; }
    }
    public GameObject deathEffect;                      // ������Ч
    public float invincibleInterval = 0.3f;             // �޵�ʱ��
    public float damgeIdleInterval = 0.3f;              // ����ͣ��ʱ��
    public float knockbackResistance;                   // ���˷�����
    public float knockbackForceX;                       // ������ X
    public float knockbackForceY;                       // ������ Y

    public float damage;                                // �˺�ֵ
    public float expToGive;                             // ���辭��ֵ

    [Header("�ƶ�")]
    public float speed;                                 // �ƶ��ٶ�

    protected virtual void Start()
    {
        currentHealthPoint = maxHealthPoint;
    }
}
