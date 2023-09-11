using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ghost : Enemy
{
    public static Ghost instance;

    [SerializeField] private bool _bDamaging;   // 是否受伤
    private Blink _blink;
    private SpriteRenderer _sprite;
    private Rigidbody2D _rb;

    public Image bossHealthImage;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    protected override void Start()
    {
        base.Start();

        _blink = GetComponent<Blink>();
        _sprite = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            // 玩家武器
            if (collision.tag == "Weapon" && !_bDamaging)
            {
                Weapon weapon = collision.GetComponent<Weapon>();

                // 受伤
                currentHealthPoint -= weapon.damage;
                StartCoroutine(Damager());

                // 击退
                float knockbackForceX = weapon.knockbackForceX * (1.0f - knockbackResistance);
                float knockbackForceY = weapon.knockbackForceY * (1.0f - knockbackResistance);
                _rb.velocity = new Vector2(0.0f, _rb.velocity.y);  // 速度置 0
                if (collision.transform.position.x < transform.position.x)
                {
                    _rb.AddForce(new Vector2(knockbackForceX, knockbackForceY), ForceMode2D.Force);
                }
                else if (collision.transform.position.x > transform.position.x)
                {
                    _rb.AddForce(new Vector2(-knockbackForceX, knockbackForceY), ForceMode2D.Force);
                }

                bossHealthImage.fillAmount = GetComponent<Enemy>().CurrentHealthPoint / GetComponent<Enemy>().maxHealthPoint;

                if (currentHealthPoint <= 0)
                {
                    currentHealthPoint = 0;
                    bossHealthImage.fillAmount = GetComponent<Enemy>().CurrentHealthPoint / GetComponent<Enemy>().maxHealthPoint;

                    // 触发死亡特效
                    Instantiate(deathEffect, transform.position, Quaternion.identity);

                    // 给予玩家经验值
                    Experience.instance.AddExp(expToGive);

                    // 销毁
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 受伤特效协程
    /// </summary>
    private IEnumerator Damager()
    {
        _bDamaging = true;
        _sprite.material = _blink.blink;    // 受伤材质
        yield return new WaitForSeconds(invincibleInterval);
        _bDamaging = false;
        _sprite.material = _blink.original; // 还原材质
    }
}
