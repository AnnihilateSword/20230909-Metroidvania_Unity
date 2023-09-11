using System.Collections;
using UnityEngine;

public class Skeleton : Enemy
{
    public float idleTime = 2.0f;                       // 闲置时间
    public Transform leftPatrolPoint;                   // 左巡逻点
    public Transform rightPatrolPoint;                  // 右左巡逻点
    [SerializeField] private bool _bIsStatic = false;   // 是否静止
    [SerializeField] private bool _bIsRun = false;      // 是否奔跑
    [SerializeField] private bool _bIsPatrol = true;    // 是否巡逻
    [SerializeField] private bool _bGoToLeft = true;    // 是否去左边指定点巡逻
    [SerializeField] private bool _bGoToRight = false;  // 是否去右边指定点巡逻
    [SerializeField] private bool _bIsRunRight = false; // 是否向右走
    public Transform groundCheck, wallCheck;
    [SerializeField] private bool _bCheckedPit, _bCheckedGround, _bCheckedWall;
    public float checkRadius = 0.5f;                    // 环境检测半径
    public LayerMask whatIsGround, whatIsWall;

    private SpriteRenderer _sprite;
    private Blink _blink;
    private Rigidbody2D _rb;
    private Animator _anim;

    [SerializeField] private bool _bDamaging;   // 是否受伤


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
            //_rb.constraints = RigidbodyConstraints2D.FreezeAll;  // 静止时冻结所有轴
        }
        // 左右巡逻
        else if (_bIsPatrol && !_bDamaging)
        {
            _anim.SetBool("Run", true);
            //_rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // 只冻结 Z轴

            if (_bGoToLeft)
            {
                // 检查是否走反了方向（玩家把敌人打下台阶的情况）
                if (transform.position.x < leftPatrolPoint.position.x)
                {
                    _bGoToLeft = false;
                    _bGoToRight = true;
                    _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
                    // 静止并转向
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
                    // 静止并转向
                    if (!_bIsStatic)
                    {
                        _bIsStatic = true;
                        StartCoroutine(Idler());
                    }
                }
            }
            else if (_bGoToRight)
            {
                // 检查是否走反了方向（玩家把敌人打下台阶的情况）
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
        // 奔跑
        else if (_bIsRun && !_bDamaging)
        {
            _anim.SetBool("Run", true);
            //_rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // 只冻结 Z轴
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
            // 玩家武器
            if (collision.tag == "Weapon" && !_bDamaging)
            {
                Weapon weapon = collision.GetComponent<Weapon>();

                // 受伤

                // 播放音效
                AudioManager.instance.PlayAudio(AudioManager.instance.skeletonDamage);

                currentHealthPoint -= weapon.damage;
                StartCoroutine(Damager());
                StartCoroutine(DamageIdler());

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

                // 敌人死亡
                if (currentHealthPoint <= 0)
                {
                    // 播放音效
                    AudioManager.instance.PlayAudio(AudioManager.instance.skeletonDeath);

                    currentHealthPoint = 0;

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

    /// <summary>
    /// 受伤停顿协程
    /// </summary>
    private IEnumerator DamageIdler()
    {
        _bIsStatic = true;
        _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
        yield return new WaitForSeconds(damgeIdleInterval);
        _bIsStatic = false;
    }

    /// <summary>
    /// 静止协程
    /// </summary>
    private IEnumerator Idler()
    {
        yield return new WaitForSeconds(idleTime);
        _bIsStatic = false;
        Flip();
    }

    /// <summary>
    /// 转向
    /// </summary>
    private void Flip()
    {
        _bIsRunRight = !_bIsRunRight;
        transform.localScale *= new Vector2(-1.0f, 1.0f);
    }


    /// <summary>
    /// Gizmos 绘制
    /// </summary>
    private void OnDrawGizmos()
    {
        // 地面检测
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
    }
}
