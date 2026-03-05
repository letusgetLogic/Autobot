using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    /// <summary>
    /// Button click calls.
    /// </summary>
    public void OnButtonClick()
    {
        if (GameManager.Instance &&
            GameManager.Instance.CurrentGame != null)
        {
            GameManager.Instance.CurrentGame.State = GameState.EndOfGame;
        }

        SceneManager.LoadScene("Menu");
    }
}

