using System;
using UnityEngine;

public class GameSession
{
    public int Seed { get; private set; }

    public void ValidateBattleResult(
        BattleResult local,
        uint remoteHash
    )
    {
        uint localHash = DeterminismHash.HashBattleResult(
            local.UnitSnapshots,
            local.Winner
        );

        if (localHash != remoteHash)
            HandleDesync();
    }

    public void HandleDesync()
    {
        Debug.LogError("DESYNC DETECTED");
        // disconnect / report / replay save
    }

    internal void HandleDisconnect()
    {
        throw new NotImplementedException();
    }
}
