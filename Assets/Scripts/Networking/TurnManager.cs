using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class TurnManager
{
    private int currentTurn;
    private TurnLockState state;

    private float timer;
    private int retryCount;

    private uint localHash;
    private uint remoteHash;

    private List<int> cachedActions;

    private NetworkManager net;
    private GameSession session;

    private uint localBattleHash;
    private uint remoteBattleHash;
    private BattleResult cachedBattleResult;

    /// <summary>
    /// Submits the player's actions for the current turn. This will start the lockstep process, where both players exchange hashes and confirmations to ensure that they are in sync. 
    /// If the lockstep process fails, the session will handle the desync or disconnect as needed.
    /// </summary>
    /// <param name="actions"></param>
    public void SubmitTurn(List<int> actions)
    {
        if (state != TurnLockState.Idle)
            return;

        cachedActions = new List<int>(actions);

        localHash = DeterminismHash.HashTurnInput(
            currentTurn,
            session.Seed,
            actions
        );

        net.SendTurnInput(currentTurn, session.Seed, actions);

        state = TurnLockState.WaitingForTurnHash;
        timer = 0f;
    }

    /// <summary>
    /// On receiving a turn hash from the remote player, this method compares it with the local hash. If they match, it sends an acknowledgment and waits for confirmation. 
    /// If they don't match, it sends a desync message and handles the desync scenario.
    /// </summary>
    /// <param name="hash"></param>
    public void OnRemoteTurnHash(uint hash)
    {
        remoteHash = hash;

        if (remoteHash != localHash)
        {
            net.SendDesync(currentTurn);
            state = TurnLockState.Failed;
            session.HandleDesync();
            return;
        }

        net.SendTurnAck(currentTurn);
        state = TurnLockState.WaitingForTurnAck;
        ResetTimeout();
    }

    /// <summary>
    /// On receiving a turn acknowledgment from the remote player, this method confirms the turn and advances to the next turn. 
    /// It also resets the lockstep state to idle for the next turn.
    /// </summary>
    public void OnTurnAckReceived()
    {
        state = TurnLockState.Confirmed;
        AdvanceTurn();
    }

    /// <summary>
    /// On receiving a battle result acknowledgment from the remote player, this method applies the battle result and advances to the next turn.
    /// </summary>
    public void OnBattleResultAckReceived()
    {
        ApplyBattleResult(cachedBattleResult);
        AdvanceTurn();
    }

    /// <summary>
    /// Applies the battle result to the game state. This method should update the game state based on the results of the battle simulation, such as updating player health, unit states, and any other relevant information.
    /// </summary>
    /// <param name="cachedBattleResult"></param>
    private void ApplyBattleResult(BattleResult cachedBattleResult)
    {
        
    }

    /// <summary>
    /// Advances to the next turn by incrementing the current turn counter and resetting the lockstep state to idle. 
    /// This method is called after a turn is confirmed or after applying the battle result, preparing the system for the next turn's input.
    /// </summary>
    private void AdvanceTurn()
    {
        currentTurn++;
        Debug.Log($"Turn {currentTurn} confirmed");

        state = TurnLockState.Idle;
        ResetTimeout();
    }

    /// <summary>
    /// On receiving turn input from the remote player, this method validates the input against the expected actions for the current turn. 
    /// If the input is valid, it proceeds with the lockstep process.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="expectedActions"></param>
    public void OnTurnInputReceived(
        DataStreamReader reader,
        List<int> expectedActions
    )
    {
        bool valid = net.ReceiveTurnInput(
            reader,
            currentTurn,
            session.Seed,
            expectedActions
        );

        if (!valid)
        { 
            net.SendDesync(currentTurn);
            state = TurnLockState.Failed;
            session.HandleDesync();
            return;
        }
    }

    /// <summary>
    /// On confirming the turn, this method runs the deterministic battle simulation based on the current game state and the actions taken by both players.
    /// </summary>
    private void OnTurnConfirmed()
    {
        state = TurnLockState.BattleSimulating;

        cachedBattleResult = BattleSimulator.RunDeterministicBattle();

        localBattleHash = DeterminismHash.HashBattleResult(
            currentTurn,
            cachedBattleResult
        );

        net.SendBattleResultHash(currentTurn, localBattleHash);

        state = TurnLockState.WaitingForBattleResultHash;
        ResetTimeout();
    }

    /// <summary>
    /// On receiving a battle result hash from the remote player, this method compares it with the local battle hash. 
    /// If they match, it sends an acknowledgment and waits for confirmation.
    /// </summary>
    /// <param name="hash"></param>
    public void OnBattleResultHashReceived(uint hash)
    {
        remoteBattleHash = hash;

        if (remoteBattleHash != localBattleHash)
        {
            net.SendDesync(currentTurn);
            state = TurnLockState.Failed;
            session.HandleDesync();
            return;
        }

        net.SendBattleResultAck(currentTurn);
        state = TurnLockState.WaitingForBattleResultAck;
        ResetTimeout();
    }

    /// <summary>
    /// This method should be called regularly (e.g., in the Update loop) to check for timeouts in the lockstep process. 
    /// If a timeout occurs, it will trigger a retry of the current step in the lockstep process.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Tick(float deltaTime)
    {
        if (state == TurnLockState.Confirmed ||
            state == TurnLockState.Idle ||
            state == TurnLockState.Failed)
            return;

        timer += deltaTime;

        if (timer >= LockstepConfig.TURN_TIMEOUT)
        {
            HandleTimeout();
            timer = 0f;
        }
    }

    /// <summary>
    /// Handles a timeout in the lockstep process by incrementing the retry count and checking if it exceeds the maximum allowed retries.
    /// </summary>
    private void HandleTimeout()
    {
        retryCount++;

        if (retryCount > LockstepConfig.MAX_RETRIES)
        {
            Debug.LogError("Lockstep failed – max retries exceeded");
            state = TurnLockState.Failed;
            session.HandleDisconnect();
            return;
        }

        Debug.Log($"Resending turn {currentTurn}, retry {retryCount}");

        Resend();
    }

    /// <summary>
    /// Resends the necessary data based on the current state of the lockstep process. This method is called when a timeout occurs, allowing the system to attempt to recover from potential packet loss or delays by resending the relevant information to the remote player.
    /// </summary>
    private void Resend()
    {
        switch (state)
        {
            case TurnLockState.WaitingForTurnHash:
                net.SendTurnInput(currentTurn, session.Seed, cachedActions);
                break;

            case TurnLockState.WaitingForTurnAck:
                net.SendTurnHash(currentTurn, localHash);
                break;

            case TurnLockState.WaitingForBattleResultHash:
                net.SendBattleResultHash(currentTurn, localBattleHash);
                break;

            case TurnLockState.WaitingForBattleResultAck:
                net.SendBattleResultHash(currentTurn, localBattleHash);
                break;
        }
    }

    /// <summary>
    /// Resets the timeout timer and retry count. This method is called after successfully receiving the expected data from the remote player, allowing the lockstep process to continue without triggering unnecessary retries or timeouts.
    /// </summary>
    private void ResetTimeout()
    {
        timer = 0f;
        retryCount = 0;
    }

    /// <summary>
    /// Resets the lockstep state to idle and clears any cached data. This method can be called when starting a new game session or after handling a desync or disconnect scenario, ensuring that the turn manager is in a clean state for the next round of gameplay.
    /// </summary>
    public void ResetLockstep()
    {
        state = TurnLockState.Idle;
        retryCount = 0;
        timer = 0f;
    }

}
