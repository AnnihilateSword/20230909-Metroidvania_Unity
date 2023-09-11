using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    public float maxHealth = 100.0f;            // �������ֵ
    [SerializeField] private float _health;     // ��ǰ����ֵ
    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }

    public float invincibleInterval;            // �޵�ʱ��
    public float dmageInterval = 0.3f;          // ����ͣ��ʱ��
    public float knockbackResistance;           // ���˷�����
    public float invincleBlinkInterval = 0.05f; // �޵���˸���

    [Header("UI")]
    public Image healthImage;
    public Text healthText;

    [SerializeField] private bool _bDamaging;   // �Ƿ�����
    [SerializeField] private bool _bInvincible; // �Ƿ��޵�
    public bool BDamaging
    {
        get { return _bDamaging; }
    }

    private SpriteRenderer _sprite;
    private Blink _blink;                               // ������˸
    private InvincibleBlink _invincibleBlink;           // �޵���˸
    private Coroutine _coroutineInvincibleBlink = null; // �޵���˸Э��

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

        // ��ֹ�����������ֵ
        if (_health > maxHealth)
        {
            _health = maxHealth;
        }

        // �޵���˸
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

            // ����
            _health -= collision.GetComponent<Enemy>().damage;

            // ������Ч
            AudioManager.instance.PlayAudio(AudioManager.instance.playerDamage);

            // ����
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

            // Э��
            StartCoroutine(Damager(player));
            StartCoroutine(Invincible());

            if (_health <= 0)
            {
                // Player Dead!!
                Destroy(gameObject);

                // ������Ч
                AudioManager.instance.PlayAudio(AudioManager.instance.playerDeath);

                StartCoroutine(GameOver(3.0f));
            }
        }

        if (collision.CompareTag("Flame") && !_bDamaging && !_bInvincible)
        {
            // ����

            // ������Ч
            AudioManager.instance.PlayAudio(AudioManager.instance.playerDamage);

            _health -= Ghost.instance.damage;

            // ����
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

            // Э��
            StartCoroutine(Damager(player));
            StartCoroutine(Invincible());

            if (_health <= 0)
            {
                // Player Dead!!
                Destroy(gameObject);

                // ������Ч
                AudioManager.instance.PlayAudio(AudioManager.instance.playerDeath);

                StartCoroutine(GameOver(3.0f));
            }
        }
    }

    IEnumerator Damager(PlayerController player)
    {
        _bDamaging = true;
        player.AnimationEvent_AttackEnd();  // �������ȼ��ߣ�����������ײ��û�أ�������ù��������¼�
        player.BCanMove = false;            // ����ʱ��ֹ����ƶ�������� AnimationEvent_AttackEnd ���棬������������� bcanmove = true...��
        player.BCanFlipCharacter = false;   // ����ʱ��ֹ��ҿ���ת��
        player.Anim.SetBool("Hurt", true);  // �������˶���
        _sprite.material = _blink.blink;    // ���˲���
        yield return new WaitForSeconds(dmageInterval);
        _bDamaging = false;
        player.BCanMove = true;
        player.BCanFlipCharacter = true;
        player.Anim.SetBool("Hurt", false); // �������˶���
        _sprite.material = _blink.original; // ��ԭ����
    }

    /// <summary>
    /// �޵�״̬
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
