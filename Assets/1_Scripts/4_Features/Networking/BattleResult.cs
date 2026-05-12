using System.Collections.Generic;

public struct BattleResult
{
    public int Winner;              // 0 draw, 1 P1, 2 P2
    public int Player1HealthDelta;
    public int Player2HealthDelta;
    public int[] UnitFinalStates;   // HP / alive flags

    public IReadOnlyList<SaveUnitData> UnitSnapshots { get; internal set; }
}
