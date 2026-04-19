using UnityEngine;

public class ReplayBattleButton : MonoBehaviour
{
    /// <summary>
    /// Initiates replay playback if input is not blocked and no replay is currently active.
    /// </summary>
    public void OnReplay()
    {
        if (GameManager.Instance.IsTutorialRunning || InputManager.Instance.IsBlockingInput(InputKey.ClickButtonReplay))
            return;

        if (GameManager.Instance.Replay != null)
            return;

        InputManager.Instance.BlocksInput = true;

        GameManager.Instance.PlayReplay();
    }
}
