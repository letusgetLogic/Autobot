using UnityEngine;

[CreateAssetMenu(fileName = "SoPack", menuName = "ScriptableObject/SoPack", order = 1)]
public class SoPack : ScriptableObject
{
    public SoIntVariable XpToLv2;
    public SoIntVariable XpToLv3;
    public SoIntVariable HealthPortion;

    public SoIntVariable Tier1AvaiableAtTurn;
    public SoIntVariable Tier2AvaiableAtTurn;
    public SoIntVariable Tier3AvaiableAtTurn;
    public SoIntVariable Tier4AvaiableAtTurn;
    public SoIntVariable Tier5AvaiableAtTurn;
    public SoIntVariable Tier6AvaiableAtTurn;

    public SoUnit[] UnitsTier1;
    public SoUnit[] UnitsTier2;
    public SoUnit[] UnitsTier3;
    public SoUnit[] UnitsTier4;
    public SoUnit[] UnitsTier5;
    public SoUnit[] UnitsTier6;
}

