using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void GameSuccess()
    {
        StartCoroutine(GameSuccessor(3.0f));
    }

    IEnumerator GameSuccessor(float sconds)
    {
        yield return new WaitForSeconds(sconds);
        SceneManager.LoadScene(2);
    }
}
