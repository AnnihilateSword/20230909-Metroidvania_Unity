using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    public float maxHealth = 100.0f;            // 最大生命值
    [SerializeField] private float _health;     // 当前生命值
    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }

    public float invincibleInterval;            // 无敌时间
    public float dmageInterval = 0.3f;          // 受伤停顿时间
    public float knockbackResistance;           // 击退反抗力
    public float invincleBlinkInterval = 0.05f; // 无敌闪烁间隔

    [Header("UI")]
    public Image healthImage;
    public Text healthText;

    [SerializeField] private bool _bDamaging;   // 是否受伤
    [SerializeField] private bool _bInvincible; // 是否无敌
    public bool BDamaging
    {
        get { return _bDamaging; }
    }

    private SpriteRenderer _sprite;
    private Blink _blink;                               // 受伤闪烁
    private InvincibleBlink _invincibleBlink;           // 无敌闪烁
    private Coroutine _coroutineInvincibleBlink = null; // 无敌闪烁协程

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _health = maxHealth;
        _sprite = GetComponentInParent<SpriteRenderer>();
        _blink = GetComponent<Blink>();
        _invincibleBlink = GetComponent<InvincibleBlink>();
    }

    private void Update()
    {
        // UI
        healthImage.fillAmount = _health / maxHealth;
        healthText.text = _health.ToString() + " / " + maxHealth.ToString();

        // 防止超过最大生命值
        if (_health > maxHealth)
        {
            _health = maxHealth;
        }

        // 无敌闪烁
        if (_bInvincible && !_bDamaging)
        {
            if (_coroutineInvincibleBlink == null)
            {
                _coroutineInvincibleBlink = StartCoroutine(InvincibleBlink());
            }
        }
        else
        {
            if (_coroutineInvincibleBlink != null)
            {
                StopCoroutine(InvincibleBlink());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !_bDamaging && !_bInvincible)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
            PlayerController player = PlayerController.instance;

            // 受伤
            _health -= collision.GetComponent<Enemy>().damage;

            // 播放音效
            AudioManager.instance.PlayAudio(AudioManager.instance.playerDamage);

            // 击退
            float knockbackForceX = enemy.knockbackForceX * (1.0f - knockbackResistance);
            float knockbackForceY = enemy.knockbackForceY * (1.0f - knockbackResistance);

            rb.velocity = new Vector2(0.0f, 0.0f);

            if (collision.transform.position.x < transform.position.x)
            {
                rb.AddForce(new Vector2(knockbackForceX, knockbackForceY), ForceMode2D.Force);
            }
            else if (collision.transform.position.x > transform.position.x)
            {
                rb.AddForce(new Vector2(-knockbackForceX, knockbackForceY), ForceMode2D.Force);
            }

            // 协程
            StartCoroutine(Damager(player));
            StartCoroutine(Invincible());

            if (_health <= 0)
            {
                // Player Dead!!
                Destroy(gameObject);

                // 播放音效
                AudioManager.instance.PlayAudio(AudioManager.instance.playerDeath);

                StartCoroutine(GameOver(3.0f));
            }
        }

        if (collision.CompareTag("Flame") && !_bDamaging && !_bInvincible)
        {
            // 受伤

            // 播放音效
            AudioManager.instance.PlayAudio(AudioManager.instance.playerDamage);

            _health -= Ghost.instance.damage;

            // 击退
            float knockbackForceX = Ghost.instance.knockbackForceX * (1.0f - knockbackResistance);
            float knockbackForceY = Ghost.instance.knockbackForceY * (1.0f - knockbackResistance);

            Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
            rb.velocity = new Vector2(0.0f, 0.0f);

            if (collision.transform.position.x < transform.position.x)
            {
                rb.AddForce(new Vector2(knockbackForceX, knockbackForceY), ForceMode2D.Force);
            }
            else if (collision.transform.position.x > transform.position.x)
            {
                rb.AddForce(new Vector2(-knockbackForceX, knockbackForceY), ForceMode2D.Force);
            }

            PlayerController player = PlayerController.instance;

            // 协程
            StartCoroutine(Damager(player));
            StartCoroutine(Invincible());

            if (_health <= 0)
            {
                // Player Dead!!
                Destroy(gameObject);

                // 播放音效
                AudioManager.instance.PlayAudio(AudioManager.instance.playerDeath);

                StartCoroutine(GameOver(3.0f));
            }
        }
    }

    IEnumerator Damager(PlayerController player)
    {
        _bDamaging = true;
        player.AnimationEvent_AttackEnd();  // 受伤优先级高，避免武器碰撞盒没关，这里调用攻击结束事件
        player.BCanMove = false;            // 受伤时禁止玩家移动（这个在 AnimationEvent_AttackEnd 下面，这个方法里面有 bcanmove = true...）
        player.BCanFlipCharacter = false;   // 受伤时禁止玩家控制转向
        player.Anim.SetBool("Hurt", true);  // 触发受伤动画
        _sprite.material = _blink.blink;    // 受伤材质
        yield return new WaitForSeconds(dmageInterval);
        _bDamaging = false;
        player.BCanMove = true;
        player.BCanFlipCharacter = true;
        player.Anim.SetBool("Hurt", false); // 结束受伤动画
        _sprite.material = _blink.original; // 还原材质
    }

    /// <summary>
    /// 无敌状态
    /// </summary>
    IEnumerator Invincible()
    {
        _bInvincible = true;
        yield return new WaitForSeconds(invincibleInterval);
        _bInvincible = false;
    }

    IEnumerator InvincibleBlink()
    {
        _sprite.material = _invincibleBlink.blink;
        yield return new WaitForSeconds(invincleBlinkInterval);
        _sprite.material = _invincibleBlink.original;
        _coroutineInvincibleBlink = null;
    }

    IEnumerator GameOver(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(3);
    }
}
