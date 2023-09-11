using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;                            // 敌人名称
    public float maxHealthPoint;                        // 最大生命值
    [SerializeField] protected float currentHealthPoint;  // 当前生命值
    public float CurrentHealthPoint
    {
        get { return currentHealthPoint; }
        set { currentHealthPoint = value; }
    }
    public GameObject deathEffect;                      // 死亡特效
    public float invincibleInterval = 0.3f;             // 无敌时间
    public float damgeIdleInterval = 0.3f;              // 受伤停顿时间
    public float knockbackResistance;                   // 击退反抗力
    public float knockbackForceX;                       // 击退力 X
    public float knockbackForceY;                       // 击退力 Y

    public float damage;                                // 伤害值
    public float expToGive;                             // 给予经验值

    [Header("移动")]
    public float speed;                                 // 移动速度

    protected virtual void Start()
    {
        currentHealthPoint = maxHealthPoint;
    }
}
