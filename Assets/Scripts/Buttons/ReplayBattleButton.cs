using UnityEngine;

public class ReplayBattleButton : MonoBehaviour
{
    /// <summary>
    /// Initiates replay playback if input is not blocked and no replay is currently active.
    /// </summary>
    public void OnReplay()
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (GameManager.Instance.Replay != null)
            return;

        GameManager.Instance.IsBlockingInput = true;

        GameManager.Instance.PlayReplay();
    }
}
