using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        GameManager.Instance.EndGame();
        Time.timeScale = 1;
    }
}

