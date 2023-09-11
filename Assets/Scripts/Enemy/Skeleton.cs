using System.Collections;
using UnityEngine;

public class Skeleton : Enemy
{
    public float idleTime = 2.0f;                       // ����ʱ��
    public Transform leftPatrolPoint;                   // ��Ѳ�ߵ�
    public Transform rightPatrolPoint;                  // ����Ѳ�ߵ�
    [SerializeField] private bool _bIsStatic = false;   // �Ƿ�ֹ
    [SerializeField] private bool _bIsRun = false;      // �Ƿ���
    [SerializeField] private bool _bIsPatrol = true;    // �Ƿ�Ѳ��
    [SerializeField] private bool _bGoToLeft = true;    // �Ƿ�ȥ���ָ����Ѳ��
    [SerializeField] private bool _bGoToRight = false;  // �Ƿ�ȥ�ұ�ָ����Ѳ��
    [SerializeField] private bool _bIsRunRight = false; // �Ƿ�������
    public Transform groundCheck, wallCheck;
    [SerializeField] private bool _bCheckedPit, _bCheckedGround, _bCheckedWall;
    public float checkRadius = 0.5f;                    // �������뾶
    public LayerMask whatIsGround, whatIsWall;

    private SpriteRenderer _sprite;
    private Blink _blink;
    private Rigidbody2D _rb;
    private Animator _anim;

    [SerializeField] private bool _bDamaging;   // �Ƿ�����


    protected override void Start()
    {
        base.Start();

        _sprite = GetComponent<SpriteRenderer>();
        _blink = GetComponent<Blink>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (_bIsStatic)
        {
            _anim.SetBool("Run", false);
            //_rb.constraints = RigidbodyConstraints2D.FreezeAll;  // ��ֹʱ����������
        }
        // ����Ѳ��
        else if (_bIsPatrol && !_bDamaging)
        {
            _anim.SetBool("Run", true);
            //_rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // ֻ���� Z��

            if (_bGoToLeft)
            {
                // ����Ƿ��߷��˷�����Ұѵ��˴���̨�׵������
                if (transform.position.x < leftPatrolPoint.position.x)
                {
                    _bGoToLeft = false;
                    _bGoToRight = true;
                    _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
                    // ��ֹ��ת��
                    if (!_bIsStatic)
                    {
                        _bIsStatic = true;
                        StartCoroutine(Idler());
                    }
                }

                _rb.velocity = new Vector2(-speed * Time.fixedDeltaTime * 100.0f, 0.0f);

                if (Vector2.Distance(transform.position, leftPatrolPoint.position) < 0.2f)
                {
                    _bGoToLeft = false;
                    _bGoToRight = true;
                    _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
                    // ��ֹ��ת��
                    if (!_bIsStatic)
                    {
                        _bIsStatic = true;
                        StartCoroutine(Idler());
                    }
                }
            }
            else if (_bGoToRight)
            {
                // ����Ƿ��߷��˷�����Ұѵ��˴���̨�׵������
                if (transform.position.x > rightPatrolPoint.position.x)
                {
                    _bGoToLeft = true;
                    _bGoToRight = false;
                    _rb.velocity = new Vector2(0.0f, _rb.velocity.y);

                    if (!_bIsStatic)
                    {
                        _bIsStatic = true;
                        StartCoroutine(Idler());
                    }
                }

                _rb.velocity = new Vector2(speed * Time.fixedDeltaTime * 100.0f, 0.0f);

                if (Vector2.Distance(transform.position, rightPatrolPoint.position) < 0.2f)
                {
                    _bGoToLeft = true;
                    _bGoToRight = false;
                    _rb.velocity = new Vector2(0.0f, _rb.velocity.y);

                    if (!_bIsStatic)
                    {
                        _bIsStatic = true;
                        StartCoroutine(Idler());
                    }
                }
            }
        }
        // ����
        else if (_bIsRun && !_bDamaging)
        {
            _anim.SetBool("Run", true);
            //_rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // ֻ���� Z��
            if (_bIsRunRight)
            {
                _rb.velocity = new Vector2(speed * Time.fixedDeltaTime * 100.0f, 0.0f);
            }
            else
            {
                _rb.velocity = new Vector2(-speed * Time.fixedDeltaTime * 100.0f, 0.0f);
            }
        }
    }

    private void Update()
    {
        _bCheckedGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        _bCheckedWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsWall);
        _bCheckedPit = !_bCheckedGround;

        if ((_bCheckedPit || _bCheckedWall) && !_bIsPatrol)
        {
            if (!_bIsStatic)
            {
                _bIsStatic = true;
                _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
                StartCoroutine(Idler());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            // �������
            if (collision.tag == "Weapon" && !_bDamaging)
            {
                Weapon weapon = collision.GetComponent<Weapon>();

                // ����

                // ������Ч
                AudioManager.instance.PlayAudio(AudioManager.instance.skeletonDamage);

                currentHealthPoint -= weapon.damage;
                StartCoroutine(Damager());
                StartCoroutine(DamageIdler());

                // ����
                float knockbackForceX = weapon.knockbackForceX * (1.0f - knockbackResistance);
                float knockbackForceY = weapon.knockbackForceY * (1.0f - knockbackResistance);
                _rb.velocity = new Vector2(0.0f, _rb.velocity.y);  // �ٶ��� 0
                if (collision.transform.position.x < transform.position.x)
                {
                    _rb.AddForce(new Vector2(knockbackForceX, knockbackForceY), ForceMode2D.Force);
                }
                else if (collision.transform.position.x > transform.position.x)
                {
                    _rb.AddForce(new Vector2(-knockbackForceX, knockbackForceY), ForceMode2D.Force);
                }

                // ��������
                if (currentHealthPoint <= 0)
                {
                    // ������Ч
                    AudioManager.instance.PlayAudio(AudioManager.instance.skeletonDeath);

                    currentHealthPoint = 0;

                    // ����������Ч
                    Instantiate(deathEffect, transform.position, Quaternion.identity);

                    // ������Ҿ���ֵ
                    Experience.instance.AddExp(expToGive);

                    // ����
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// ������ЧЭ��
    /// </summary>
    private IEnumerator Damager()
    {
        _bDamaging = true;
        _sprite.material = _blink.blink;    // ���˲���
        yield return new WaitForSeconds(invincibleInterval);
        _bDamaging = false;
        _sprite.material = _blink.original; // ��ԭ����
    }

    /// <summary>
    /// ����ͣ��Э��
    /// </summary>
    private IEnumerator DamageIdler()
    {
        _bIsStatic = true;
        _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
        yield return new WaitForSeconds(damgeIdleInterval);
        _bIsStatic = false;
    }

    /// <summary>
    /// ��ֹЭ��
    /// </summary>
    private IEnumerator Idler()
    {
        yield return new WaitForSeconds(idleTime);
        _bIsStatic = false;
        Flip();
    }

    /// <summary>
    /// ת��
    /// </summary>
    private void Flip()
    {
        _bIsRunRight = !_bIsRunRight;
        transform.localScale *= new Vector2(-1.0f, 1.0f);
    }


    /// <summary>
    /// Gizmos ����
    /// </summary>
    private void OnDrawGizmos()
    {
        // ������
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
    }
}
