using UnityEngine;

[CreateAssetMenu(fileName = "Pack", menuName = "ScriptableObject/Pack", order = 1)]
public class SoPack : ScriptableObject
{
    [Header("General Settings")]
    public SoIntVariable XpToLv2;
    public SoIntVariable XpToLv3;
    public SoIntVariable MaxXP; // maxed experience point
    public SoIntVariable MaxHP; // maxed hit point
    public SoIntVariable MaxATK; // maxed attack point
    public SoIntVariable MaxENG; // maxed energy point

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

    [Header("Units Tier 1")]
    public SoUnit[] BotsTier1;
    public SoUnit[] ItemsTier1;
    [Header("Units Tier 2")]
    public SoUnit[] BotsTier2;
    public SoUnit[] ItemsTier2;
    [Header("Units Tier 3")]
    public SoUnit[] BotsTier3;
    public SoUnit[] ItemsTier3;
    [Header("Units Tier 4")]
    public SoUnit[] BotsTier4;
    public SoUnit[] ItemsTier4;
    [Header("Units Tier 5")]
    public SoUnit[] BotsTier5;
    public SoUnit[] ItemsTier5;
    [Header("Units Tier 6")]
    public SoUnit[] BotsTier6;
    public SoUnit[] ItemsTier6;

    [Header("Summoned Robots")]
    public SoUnit[] SummonedBots;

    [Header("Temporary Objects")]
    public SoUnit[] TemporaryItems;

    public SoUnit[][] Bots => new SoUnit[][]
    {
        BotsTier1,
        BotsTier2,
        BotsTier3,
        BotsTier4,
        BotsTier5,
        BotsTier6,
    };

    public SoUnit[][] Items => new SoUnit[][]
    {
        ItemsTier1,
        ItemsTier2,
        ItemsTier3,
        ItemsTier4,
        ItemsTier5,
        ItemsTier6,
    };
}

