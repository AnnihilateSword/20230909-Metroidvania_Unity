using UnityEngine;
using UnityEngine.UI;

public class Experience : MonoBehaviour
{
    public Image expImage;
    public Text expText;
    public Text levelText;
    public float currentExperience, expToNextLevel;

    private bool _bLevelUp = false;

    public static Experience instance;

    [SerializeField] private uint _currentLevel = 1;     // ��ǰ�ȼ�

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        expImage.fillAmount = currentExperience / expToNextLevel;
        expText.text = currentExperience.ToString() + " / " + expToNextLevel.ToString();
    }

    /// <summary>
    /// ���Ӿ���
    /// </summary>
    public void AddExp(float exp)
    {
        currentExperience += exp;

        // ����������while�Ǳ���������鳬��1����
        while (currentExperience >= expToNextLevel)
        {
            _bLevelUp = true;
            // �������־���
            float excessExp = currentExperience - expToNextLevel;

            expToNextLevel *= 2;
            currentExperience = excessExp;
            Debug.Log("Level!!!");

            // ����
            _currentLevel++;

            // ��������...

            // ���������������
            PlayerHealth.instance.maxHealth += 50.0f;  // ��ʱ�� 50�� Ѫ��
        }

        if (_bLevelUp)  // ��������
        {
            _bLevelUp = false;
            // ������Ч
            AudioManager.instance.PlayAudio(AudioManager.instance.playerLevelUp);

            // �����ظ���Ѫ
            PlayerHealth.instance.Health = PlayerHealth.instance.maxHealth;
        }

        expImage.fillAmount = currentExperience / expToNextLevel;
        expText.text = currentExperience.ToString() + " / " + expToNextLevel.ToString();
        levelText.text = "Level" + _currentLevel;
    }
}
