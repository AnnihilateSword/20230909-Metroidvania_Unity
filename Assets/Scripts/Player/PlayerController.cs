using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�ƶ�")]
    public float speed = 5.0f;                                  // �ƶ��ٶ�
    public float shadowModeSpeed = 8.0f;                        // ��Ӱģʽ�ٶ�
    [SerializeField] private float _currentSpeed;               // ��ǰ�ٶ�
    public float jumpHeight = 10.0f;                            // ��Ծ�߶�
    public uint airJumpCount = 1;                               // ������Ծ����

    [Header("�������")]
    public Transform checkGround;                               // ����������λ��
    public float checkGroundRadius = 0.55f;                     // ������뾶
    public LayerMask whatIsGround;                              // ����ͼ��
    public LayerMask whatIsWall;                                // ǽ��ͼ��

    [Header("����")]
    public Weapon weapon;                                       // ����
    public float attackMoveX = 0.2f;                            // ����ʱ X �����ƶ�
    public GameObject arrowPrefab;                              // ��ʸ
    public Transform arrowSpwan;                                // ��ʸ

    [Header("BUFF")]
    [Space(20)]

    private bool _bJump = false;
    [SerializeField] private bool _bIsGrounded = false;         // �Ƿ��ڵ���
    [SerializeField] private uint _airJumpCountRef;             // ʣ�������Ծ����

    [Header("״̬")]
    [SerializeField] private bool _bAttacking = false;          // �Ƿ��ڹ���
    [SerializeField] private bool _bJumping = false;            // �Ƿ�����Ծ
    [SerializeField] private bool _bCanMove = true;             // �Ƿ���ƶ�
    [SerializeField] private bool _bCanFlipCharacter = true;    // �Ƿ��ת��
    [SerializeField] private bool _bIsShadowing = false;        // ��Ӱģʽ

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


    private Rigidbody2D _rb;                            // �������
    private Animator _anim;                             // ������

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
    /// �ƶ�
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

            // �ƶ�ʱת��
            FlipCharacter();
        }
    }

    /// <summary>
    /// ��ɫת��
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
    /// ��⴦��
    /// </summary>
    public void CheckProcess()
    {
        CheckKey();
        CheckEnvironment();
        CheckAnim();
    }

    /// <summary>
    /// �������
    /// </summary>
    public void CheckKey()
    {
        // ��Ծ
        if (Input.GetKeyDown(KeyCode.K) && _airJumpCountRef > 0)
        {
            _bJump = true;
            _airJumpCountRef--;
        }

        // ��ͨ����
        if (Input.GetKeyDown(KeyCode.J) && !_bAttacking && !playerHealth.BDamaging)
        {
            // ������Ч
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

        // Զ�̹���
        if (Input.GetKeyDown(KeyCode.U) && !playerHealth.BDamaging)
        {
            if (arrowPrefab != null)
            {
                if (ItemAccount.instance.arrow > 0)
                {
                    // ʹ�ü�ʸ
                    ItemAccount.instance.UseArrow(1);

                    // ������Ч
                    AudioManager.instance.PlayAudio(AudioManager.instance.arrow);

                    GameObject arrow = Instantiate(arrowPrefab, arrowSpwan.position, Quaternion.identity);
                    ArrowWeapon arrowWeapon = arrow.GetComponent<ArrowWeapon>();
                    Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

                    // ����
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
                    // ��ҩ�ľ�
                    // ...
                }
            }
        }

        // ��Ӱģʽ�����أ�
        if (Input.GetKeyDown(KeyCode.P))
        {
            _bIsShadowing = !_bIsShadowing;
            if (_bIsShadowing)
            {
                Debug.Log("��Ӱģʽ�����ã�");
                _currentSpeed = shadowModeSpeed;  // �����ٶ�
                Shadows.instance.BEnableShadow = true;
            }
            else
            {
                Debug.Log("��Ӱģʽ���رգ�");
                _currentSpeed = speed;
                Shadows.instance.BEnableShadow = false;
            }
        }
    }

    /// <summary>
    /// �������
    /// </summary>
    public void CheckEnvironment()
    {
        bool isGrounded = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, whatIsGround);
        bool isWall = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, whatIsWall);
        _bIsGrounded = (isGrounded || isWall);

        if (_bIsGrounded)  // �ڵ���
        {
            _airJumpCountRef = airJumpCount;
            // �������ת��
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
            // �ڿ��в���ת��
            _bCanFlipCharacter = false;
            if (!_bAttacking)
            {
                // animation
                _anim.SetBool("Jump", true);
            }
        }
    }

    /// <summary>
    /// �����������
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
    /// ��Ծ
    /// </summary>
    public void Jump()
    {
        if (_bJump && _bCanMove)
        {
            _bJump = false;
            _bJumping = true;

            // ��Ծʱ����ת��
            _bCanFlipCharacter = false;

            _rb.velocity = new Vector2(_rb.velocity.x, jumpHeight * Time.fixedDeltaTime * 100);

            // animation
            _anim.SetBool("Jump", true);
        }
    }

    /// <summary>
    /// Gizmos ����
    /// </summary>
    private void OnDrawGizmos()
    {
        // ������
        Gizmos.DrawWireSphere(checkGround.position, checkGroundRadius);
    }

    /// <summary>
    /// ������������
    /// </summary>
    public void AnimationEvent_AttackEnd()
    {
        _bAttacking = false;
        _bCanMove = true;
        _bCanFlipCharacter = true;

        weapon.GetComponent<BoxCollider2D>().enabled = false;
    }
}
