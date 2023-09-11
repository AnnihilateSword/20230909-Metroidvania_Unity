using UnityEngine;

public class Shadows : MonoBehaviour
{
    public GameObject shadow;
    public float shadowTime;    // ��ʾʱ��
    private float shadowTimeCD;
    [SerializeField] private bool _bEnableShadow = false;
    public bool BEnableShadow
    {
        get { return _bEnableShadow; }
        set { _bEnableShadow = value; }
    }

    public static Shadows instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        shadowTimeCD = shadowTime;
    }

    private void Update()
    {
        if (_bEnableShadow)
        {
            if (shadowTimeCD > 0)
            {
                shadowTimeCD -= Time.deltaTime;
            }
            else
            {
                if (GetComponent<Rigidbody2D>().velocity.x != 0.0f || GetComponent<Rigidbody2D>().velocity.y != 0.0f)
                {
                    InstantiateShadow();
                }
                shadowTimeCD = shadowTime;
            }
        }
    }

    public void InstantiateShadow()
    {
        GameObject currentShadow = Instantiate(shadow, transform.position, Quaternion.identity);
        currentShadow.transform.localScale = transform.localScale;
        currentShadow.GetComponent<SpriteRenderer>().sprite = transform.GetComponent<SpriteRenderer>().sprite;
        Destroy(currentShadow, 0.31f);  // ������ 0.3 �룬������ 0.31 ������
    }

}
