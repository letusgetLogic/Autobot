using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }
}

