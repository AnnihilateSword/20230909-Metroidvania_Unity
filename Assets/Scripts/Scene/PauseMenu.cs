using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    [SerializeField] private bool _bIsPaused = false;

    private void Awake()
    {
        _bIsPaused = false;
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        Pause();
    }

    public void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_bIsPaused)
            {
                _bIsPaused = true;
                Time.timeScale = 0.0f;
                pauseMenu.SetActive(true);
            }
            else
            {
                _bIsPaused = false;
                Time.timeScale = 1.0f;
                pauseMenu.SetActive(false);
            }
        }
    }
}
