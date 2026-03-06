using System;
using System.Collections.Generic;
using System.IO;

public static class DeterminismHash
{
    /// <summary>
    /// Hashes the input for a turn, including the turn number, random seed, and player actions.
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="seed"></param>
    /// <param name="actions"></param>
    /// <returns></returns>
    public static uint HashTurnInput(
        int turn,
        int seed,
        IReadOnlyList<int> actions
    )
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write(turn);
        writer.Write(seed);
        writer.Write(actions.Count);

        for (int i = 0; i < actions.Count; i++)
            writer.Write(actions[i]);

        return Crc32.Compute(stream.ToArray());
    }

    /// <summary>
    /// Hashes the battle result, including the winner and the final state of all units.
    /// </summary>
    /// <param name="units"></param>
    /// <param name="winner"></param>
    /// <returns></returns>
    public static uint HashBattleResult(
        IReadOnlyList<SaveUnitData> units,
        int winner
    )
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write(winner);
        writer.Write(units.Count);

        for (int i = 0; i < units.Count; i++)
        {
            //writer.Write(units[i].Id);
            //writer.Write(units[i].Hp);
        }

        return Crc32.Compute(stream.ToArray());
    }

    /// <summary>
    /// Hashes the battle result, including the winner and the final state of all units, using a custom hash function.
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static uint HashBattleResult(int turn, BattleResult result)
    {
        unchecked
        {
            uint hash = 2166136261;
            hash = (hash ^ (uint)turn) * 16777619;
            hash = (hash ^ (uint)result.Winner) * 16777619;
            hash = (hash ^ (uint)result.Player1HealthDelta) * 16777619;
            hash = (hash ^ (uint)result.Player2HealthDelta) * 16777619;

            foreach (var state in result.UnitFinalStates)
                hash = (hash ^ (uint)state) * 16777619;

            return hash;
        }
    }

    /// <summary>
    /// Hashes the battle result, including the winner and the final state of all units, using a custom hash function.
    /// </summary>
    /// <param name="unitSnapshots"></param>
    /// <param name="winner"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal static uint HashBattleResult(object unitSnapshots, int winner)
    {
        throw new NotImplementedException();
    }
}

