using UnityEngine;

public class QuitButton : MonoBehaviour
{

    /// <summary>
    /// Closes the game application.
    /// </summary>
    public void OnButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
