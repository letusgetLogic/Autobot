using UnityEngine;

[CreateAssetMenu(fileName = "Pack", menuName = "ScriptableObject/Pack", order = 1)]
public class SoPack : ScriptableObject
{
    [Header("General Settings")]
    public SoIntVariable XpToLv2;
    public SoIntVariable XpToLv3;
    public SoIntVariable MaxXP; // maxed experience point
    public SoIntVariable MaxHP; // maxed hit point
    public SoIntVariable MaxATK; // maxed attack point;
    public SoIntVariable MaxEnergy;

    public SoIntVariable ChargingEnergy;
    public SoIntVariable ChargingEnergyTeam;

    public SoTradingCurrency CurrencyData;

    [Header("Avaiable Units at which turn")]
    public SoIntVariable Tier1AvaiableAtTurn;
    public SoIntVariable Tier2AvaiableAtTurn;
    public SoIntVariable Tier3AvaiableAtTurn;
    public SoIntVariable Tier4AvaiableAtTurn;
    public SoIntVariable Tier5AvaiableAtTurn;
    public SoIntVariable Tier6AvaiableAtTurn;

    [Header("Battle Units Tier 1")]
    public SoUnit[] BotsTier1;
    public SoUnit[] ItemsTier1;
    [Header("Battle Units Tier 2")]
    public SoUnit[] BotsTier2;
    public SoUnit[] ItemsTier2;
    [Header("Battle Units Tier 3")]
    public SoUnit[] BotsTier3;
    public SoUnit[] ItemsTier3;
    [Header("Battle Units Tier 4")]
    public SoUnit[] BotsTier4;
    public SoUnit[] ItemsTier4;
    [Header("Battle Units Tier 5")]
    public SoUnit[] BotsTier5;
    public SoUnit[] ItemsTier5;
    [Header("Battle Units Tier 6")]
    public SoUnit[] BotsTier6;
    public SoUnit[] ItemsTier6;
}

