using UnityEngine;

public class ReplayBattleButton : MonoBehaviour
{

    private StateBase state { get; set; }

    /// <summary>
    /// This sub state is used to run another states without breaking the current base state.
    /// </summary>
    public StateBase SubState { get; set; }

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
