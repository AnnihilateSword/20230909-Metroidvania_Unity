using UnityEngine;
using UnityEngine.UI;

public class ItemAccount : MonoBehaviour
{
    // Gold
    public float bank;
    public Text bankText;
    // Arrow
    public uint arrow;
    public Text arrowText;

    public static ItemAccount instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        bankText.text = bank.ToString();
    }

    /// <summary>
    /// �ռ����
    /// </summary>
    public void Money(float cashCollected)
    {
        bank += cashCollected;
        bankText.text = bank.ToString();
    }

    /// <summary>
    /// �ռ���ʸ
    /// </summary>
    public void Arraw(uint arrowCount)
    {
        arrow += arrowCount;
        arrowText.text = arrow.ToString();
    }

    /// <summary>
    /// ʹ�ü�ʸ
    /// </summary>
    public void UseArrow(uint expendArrow)
    {
        arrow -= expendArrow;
        arrowText.text = arrow.ToString();
    }
}
