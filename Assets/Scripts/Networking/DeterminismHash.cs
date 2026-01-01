using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;

public static class DeterminismHash
{
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

    internal static uint HashBattleResult(object unitSnapshots, int winner)
    {
        throw new NotImplementedException();
    }
}

