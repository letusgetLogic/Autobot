using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    /// <summary>
    /// Button click calls.
    /// </summary>
    public void OnButtonClick()
    {
        SceneManager.LoadScene("Menu");
    }
}

