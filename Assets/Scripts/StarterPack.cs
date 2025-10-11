using System.Collections.Generic;
using UnityEngine;

public class StarterPack : MonoBehaviour
{
    public static StarterPack Instance { get; private set; }

    public int XpToLv2 = 3;
    public int XpToLv3 = 6;

    public int AddHealth = 1;
    public int AddAttack = 1;

    [SerializeField] int tier1AvaiableAtTurn = 1;
    [SerializeField] int tier2AvaiableAtTurn = 3;
    [SerializeField] int tier3AvaiableAtTurn = 5;
    [SerializeField] int tier4AvaiableAtTurn = 7;
    [SerializeField] int tier5AvaiableAtTurn = 9;
    [SerializeField] int tier6AvaiableAtTurn = 11;

    public SoUnit[] UnitsTier1 = null;
    public SoUnit[] UnitsTier2 = null;
    public SoUnit[] UnitsTier3 = null;
    public SoUnit[] UnitsTier4 = null;
    public SoUnit[] UnitsTier5 = null;
    public SoUnit[] UnitsTier6 = null;

    public SoItem[] ItemsTier1 = null;
    public SoItem[] ItemsTier2 = null;
    public SoItem[] ItemsTier3 = null;
    public SoItem[] ItemsTier4 = null;
    public SoItem[] ItemsTier5 = null;
    public SoItem[] ItemsTier6 = null;

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    /// <summary>
    /// Adds units to the collection based on the specified turn and their availability tier.
    /// </summary>
    /// <remarks>This method checks the availability of units for each tier based on predefined turn
    /// thresholds. If the specified turn matches the availability turn for a particular tier, the corresponding units
    /// are added. If no match is found, no units are added.</remarks>
    /// <param name="turns">The current turn number. Determines which tier of units will be added.</param>
    public void AddUnitsByTier(int turns)
    {
        int a = tier1AvaiableAtTurn;
        int b = tier2AvaiableAtTurn;
        int c = tier3AvaiableAtTurn;
        int d = tier4AvaiableAtTurn;
        int e = tier5AvaiableAtTurn;
        int f = tier6AvaiableAtTurn;

        if (turns == a)
        {
            AddUnits(UnitsTier1);
        }
        else if (turns == b)
        {
            AddUnits(UnitsTier2);
        }
        else if (turns == c)
        {
            AddUnits(UnitsTier3);
        }
        else if (turns == d)
        {
            AddUnits(UnitsTier4);
        }
        else if (turns == e)
        {
            AddUnits(UnitsTier5);
        }
        else if (turns == f)
        {
            AddUnits(UnitsTier6);
        }
    }

    /// <summary>
    /// Adds the specified units to the game's unit collection.
    /// </summary>
    /// <remarks>This method adds all units from the provided array to the global unit collection managed by
    /// the game. Ensure that the array is not null and contains valid units before calling this method.</remarks>
    /// <param name="_tier">An array of units to be added. Each unit in the array will be added to the game's unit collection.</param>
    private void AddUnits(SoUnit[] _tier)
        {
        foreach (var unit in _tier)
        {
            GameManager.Instance.AvaiableUnits.Add(unit);
        }
    }
}

