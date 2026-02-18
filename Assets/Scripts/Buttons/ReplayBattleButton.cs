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

        GameManager.Instance.IsBlockingInput = true;
        GameManager.Instance.IsReplay = true;

        if (PhaseShopController.Instance != null)
        {
            var player = PhaseShopController.Instance.Player;

            player.SaveDataByReplay();

            CutScene.Instance.SwitchScene("PhaseBattle");
        }
        else if (PhaseBattleController.Instance != null)
        {
            CutScene.Instance.SwitchScene("PhaseBattle");
        }
    }
}
