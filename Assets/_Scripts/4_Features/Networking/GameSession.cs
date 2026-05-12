using System;
using UnityEngine;

public class GameSession
{
    public int Seed { get; private set; }

    /// <summary>
    /// Validates the battle result by comparing the hash of the local result with the hash received from the remote player. 
    /// If they do not match, it indicates a desynchronization (desync) between the two players, which can occur due to differences in game state or logic. 
    /// In such cases, appropriate actions should be taken, such as disconnecting from the game, reporting the issue, or replaying a saved state to attempt to resynchronize the game.
    /// </summary>
    /// <param name="local"></param>
    /// <param name="remoteHash"></param>
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

    /// <summary>
    /// Handles desynchronization (desync) between the local and remote game states.
    /// </summary>
    public void HandleDesync()
    {
        Debug.LogError("DESYNC DETECTED");
        // disconnect / report / replay save
    }

    /// <summary>
    /// Handles disconnection from the game, which can occur due to network issues, player actions, or as a result of desynchronization.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    internal void HandleDisconnect()
    {
        throw new NotImplementedException();
    }
}
