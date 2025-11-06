using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        GameManager.Instance.EndGame();

        Destroy(PackManager.Instance.gameObject);
        Destroy(SpawnManager.Instance.gameObject);
        Destroy(gameObject);

        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
}

