using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动")]
    public float speed = 5.0f;                                  // 移动速度
    public float shadowModeSpeed = 8.0f;                        // 残影模式速度
    [SerializeField] private float _currentSpeed;               // 当前速度
    public float jumpHeight = 10.0f;                            // 跳跃高度
    public uint airJumpCount = 1;                               // 空中跳跃次数

    [Header("环境检测")]
    public Transform checkGround;                               // 地面检测对象的位置
    public float checkGroundRadius = 0.55f;                     // 地面检测半径
    public LayerMask whatIsGround;                              // 地面图层
    public LayerMask whatIsWall;                                // 墙壁图层

    [Header("攻击")]
    public Weapon weapon;                                       // 武器
    public float attackMoveX = 0.2f;                            // 攻击时 X 方向移动
    public GameObject arrowPrefab;                              // 箭矢
    public Transform arrowSpwan;                                // 箭矢

    [Header("BUFF")]
    [Space(20)]

    private bool _bJump = false;
    [SerializeField] private bool _bIsGrounded = false;         // 是否在地面
    [SerializeField] private uint _airJumpCountRef;             // 剩余空中跳跃次数

    [Header("状态")]
    [SerializeField] private bool _bAttacking = false;          // 是否在攻击
    [SerializeField] private bool _bJumping = false;            // 是否在跳跃
    [SerializeField] private bool _bCanMove = true;             // 是否可移动
    [SerializeField] private bool _bCanFlipCharacter = true;    // 是否可转向
    [SerializeField] private bool _bIsShadowing = false;        // 残影模式

    public bool BCanFlipCharacter
    {
        get { return _bCanFlipCharacter; }
        set { _bCanFlipCharacter = value; }
    }


    public bool BCanMove
    {
        get { return _bCanMove; }
        set { _bCanMove = value; }
    }


    private Rigidbody2D _rb;                            // 刚体对象
    private Animator _anim;                             // 动画器

    public Animator Anim
    {
        get { return _anim; }
    }

    public PlayerHealth playerHealth;                   // PlayerHealth

    public static PlayerController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _airJumpCountRef = airJumpCount;
        _currentSpeed = speed;
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void Update()
    {
        CheckProcess();
    }

    /// <summary>
    /// 移动
    /// </summary>
    public void Move()
    {
        if (_bCanMove)
        {
            float horizontalVal = Input.GetAxisRaw("Horizontal");

            _rb.velocity = new Vector2(horizontalVal * _currentSpeed * Time.fixedDeltaTime * 100, _rb.velocity.y);

            // animation
            if (Mathf.Abs(_rb.velocity.x) > 0.1f)
            {
                _anim.SetBool("Run", true);
            }
            else
            {
                _anim.SetBool("Run", false);
            }

            // 移动时转向
            FlipCharacter();
        }
    }

    /// <summary>
    /// 角色转向
    /// </summary>
    public void FlipCharacter()
    {
        if (_bCanFlipCharacter)
        {
            if (_rb.velocity.x > 0.1f)
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (_rb.velocity.x < -0.1f)
            {
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
        }
    }

    /// <summary>
    /// 检测处理
    /// </summary>
    public void CheckProcess()
    {
        CheckKey();
        CheckEnvironment();
        CheckAnim();
    }

    /// <summary>
    /// 按键检测
    /// </summary>
    public void CheckKey()
    {
        // 跳跃
        if (Input.GetKeyDown(KeyCode.K) && _airJumpCountRef > 0)
        {
            _bJump = true;
            _airJumpCountRef--;
        }

        // 普通攻击
        if (Input.GetKeyDown(KeyCode.J) && !_bAttacking && !playerHealth.BDamaging)
        {
            // 播放音效
            AudioManager.instance.PlayAudio(AudioManager.instance.hit);

            _bAttacking = true;
            _bCanMove = false;
            _bCanFlipCharacter = false;

            if (transform.localScale.x > 0.01f)
            {
                _rb.velocity = new Vector2(attackMoveX, _rb.velocity.y);
            }
            else if (transform.localScale.x < -0.01f)
            {
                _rb.velocity = new Vector2(-attackMoveX, _rb.velocity.y);
            }

            _anim.SetTrigger("Attack");

            weapon.GetComponent<BoxCollider2D>().enabled = true;
        }

        // 远程攻击
        if (Input.GetKeyDown(KeyCode.U) && !playerHealth.BDamaging)
        {
            if (arrowPrefab != null)
            {
                if (ItemAccount.instance.arrow > 0)
                {
                    // 使用箭矢
                    ItemAccount.instance.UseArrow(1);

                    // 播放音效
                    AudioManager.instance.PlayAudio(AudioManager.instance.arrow);

                    GameObject arrow = Instantiate(arrowPrefab, arrowSpwan.position, Quaternion.identity);
                    ArrowWeapon arrowWeapon = arrow.GetComponent<ArrowWeapon>();
                    Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

                    // 发射
                    if (transform.localScale.x > 0.0f)
                    {
                        arrowRb.transform.localScale *= new Vector2(1.0f, 1.0f);
                        arrowRb.AddForce(new Vector2(arrowWeapon.speed, 0.0f), ForceMode2D.Impulse);
                    }
                    else if (transform.localScale.x < 0.0f)
                    {
                        arrowRb.transform.localScale *= new Vector2(-1.0f, 1.0f);
                        arrowRb.AddForce(new Vector2(-arrowWeapon.speed, 0.0f), ForceMode2D.Impulse);
                    }
                }
                else
                {
                    // 弹药耗尽
                    // ...
                }
            }
        }

        // 残影模式（开关）
        if (Input.GetKeyDown(KeyCode.P))
        {
            _bIsShadowing = !_bIsShadowing;
            if (_bIsShadowing)
            {
                Debug.Log("残影模式（启用）");
                _currentSpeed = shadowModeSpeed;  // 更改速度
                Shadows.instance.BEnableShadow = true;
            }
            else
            {
                Debug.Log("残影模式（关闭）");
                _currentSpeed = speed;
                Shadows.instance.BEnableShadow = false;
            }
        }
    }

    /// <summary>
    /// 环境检测
    /// </summary>
    public void CheckEnvironment()
    {
        bool isGrounded = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, whatIsGround);
        bool isWall = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, whatIsWall);
        _bIsGrounded = (isGrounded || isWall);

        if (_bIsGrounded)  // 在地面
        {
            _airJumpCountRef = airJumpCount;
            // 地面可以转向
            _bCanFlipCharacter = true;

            if (_rb.velocity.y < 0.1f)
            {
                _bJumping = false;

                // animation
                _anim.SetBool("Jump", false);
            }
        }
        else
        {
            // 在空中不能转向
            _bCanFlipCharacter = false;
            if (!_bAttacking)
            {
                // animation
                _anim.SetBool("Jump", true);
            }
        }
    }

    /// <summary>
    /// 动画参数检测
    /// </summary>
    public void CheckAnim()
    {
        if (_bIsGrounded && Mathf.Abs(_rb.velocity.x) < 0.1f)
        {
            _anim.SetBool("Idle", true);
        }
        else
        {
            _anim.SetBool("Idle", false);
        }
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    public void Jump()
    {
        if (_bJump && _bCanMove)
        {
            _bJump = false;
            _bJumping = true;

            // 跳跃时不能转向
            _bCanFlipCharacter = false;

            _rb.velocity = new Vector2(_rb.velocity.x, jumpHeight * Time.fixedDeltaTime * 100);

            // animation
            _anim.SetBool("Jump", true);
        }
    }

    /// <summary>
    /// Gizmos 绘制
    /// </summary>
    private void OnDrawGizmos()
    {
        // 地面检测
        Gizmos.DrawWireSphere(checkGround.position, checkGroundRadius);
    }

    /// <summary>
    /// 攻击动画结束
    /// </summary>
    public void AnimationEvent_AttackEnd()
    {
        _bAttacking = false;
        _bCanMove = true;
        _bCanFlipCharacter = true;

        weapon.GetComponent<BoxCollider2D>().enabled = false;
    }
}
