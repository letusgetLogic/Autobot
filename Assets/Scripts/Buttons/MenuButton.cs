using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        GameManager.Instance.EndGame();

        if (PackManager.Instance != null)
            Destroy(PackManager.Instance.gameObject);
        if (SpawnManager.Instance != null)
            Destroy(SpawnManager.Instance.gameObject);

        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
}

