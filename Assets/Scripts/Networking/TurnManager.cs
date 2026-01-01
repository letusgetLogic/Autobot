using System;
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
    public void OnTurnAckReceived()
    {
        state = TurnLockState.Confirmed;
        AdvanceTurn();
    }

    public void OnBattleResultAckReceived()
    {
        ApplyBattleResult(cachedBattleResult);
        AdvanceTurn();
    }

    private void ApplyBattleResult(BattleResult cachedBattleResult)
    {
        
    }

    private void AdvanceTurn()
    {
        currentTurn++;
        Debug.Log($"Turn {currentTurn} confirmed");

        state = TurnLockState.Idle;
        ResetTimeout();
    }

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

    private void ResetTimeout()
    {
        timer = 0f;
        retryCount = 0;
    }

    public void ResetLockstep()
    {
        state = TurnLockState.Idle;
        retryCount = 0;
        timer = 0f;
    }

}
