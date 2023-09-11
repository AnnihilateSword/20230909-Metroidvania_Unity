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

    [SerializeField] private uint _currentLevel = 1;     // 当前等级

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
    /// 增加经验
    /// </summary>
    public void AddExp(float exp)
    {
        currentExperience += exp;

        // 升级！（用while是避免大量经验超出1级）
        while (currentExperience >= expToNextLevel)
        {
            _bLevelUp = true;
            // 超出部分经验
            float excessExp = currentExperience - expToNextLevel;

            expToNextLevel *= 2;
            currentExperience = excessExp;
            Debug.Log("Level!!!");

            // 升级
            _currentLevel++;

            // 满级处理...

            // 升级提升玩家属性
            PlayerHealth.instance.maxHealth += 50.0f;  // 暂时加 50点 血量
        }

        if (_bLevelUp)  // 升级处理
        {
            _bLevelUp = false;
            // 播放音效
            AudioManager.instance.PlayAudio(AudioManager.instance.playerLevelUp);

            // 升级回复满血
            PlayerHealth.instance.Health = PlayerHealth.instance.maxHealth;
        }

        expImage.fillAmount = currentExperience / expToNextLevel;
        expText.text = currentExperience.ToString() + " / " + expToNextLevel.ToString();
        levelText.text = "Level" + _currentLevel;
    }
}
