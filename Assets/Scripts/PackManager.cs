using System.Collections.Generic;
using UnityEngine;

public class PackManager : MonoBehaviour
{
    public static PackManager Instance { get; private set; }

    public SoPack MyPack { get; private set; }

    public List<SoUnit> Units { get; private set; } = new List<SoUnit>();

    public int DebugID { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        DebugID = 0;
    }

    /// <summary>
    /// Initializes the pack.
    /// </summary>
    /// <param name="_selectedPack"></param>
    public void InitPack(SoPack _selectedPack)
    {
        if ( MyPack != null)
            MyPack = null;

        MyPack = _selectedPack;

        Add(MyPack.UnitsTier1);
        Add(MyPack.UnitsTier2);
        Add(MyPack.UnitsTier3);
        Add(MyPack.UnitsTier4);
        Add(MyPack.UnitsTier5);
        Add(MyPack.UnitsTier6);
    }

    /// <summary>
    /// Adds the units to the list.
    /// </summary>
    /// <param name="_units"></param>
    private void Add(SoUnit[] _units)
    {
        if (_units != null)
        {
            for (int i = 0; i < _units.Length; i++)
            {
                Units.Add(_units[i]);
            }
        }
    }

    /// <summary>
    /// Adds units to the collection based on the specified turn and their availability tier.
    /// </summary>
    /// <remarks>This method checks the availability of units for each tier based on predefined turn
    /// thresholds. If the specified turn matches the availability turn for a particular tier, the corresponding units
    /// are added. If no match is found, no units are added.</remarks>
    /// <param name="_turns">The current turn number. Determines which tier of units will be added.</param>
    public void AddUnitsByTier(int _turns)
    {
        int a = MyPack.Tier1AvaiableAtTurn.Value;
        int b = MyPack.Tier2AvaiableAtTurn.Value;
        int c = MyPack.Tier3AvaiableAtTurn.Value;
        int d = MyPack.Tier4AvaiableAtTurn.Value;
        int e = MyPack.Tier5AvaiableAtTurn.Value;
        int f = MyPack.Tier6AvaiableAtTurn.Value;

        if (_turns == a)
        {
            AddUnits(MyPack.UnitsTier1);
        }
        else if (_turns == b)
        {
            AddUnits(MyPack.UnitsTier2);
        }
        else if (_turns == c)
        {
            AddUnits(MyPack.UnitsTier3);
        }
        else if (_turns == d)
        {
            AddUnits(MyPack.UnitsTier4);
        }
        else if (_turns == e)
        {
            AddUnits(MyPack.UnitsTier5);
        }
        else if (_turns == f)
        {
            AddUnits(MyPack.UnitsTier6);
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
            Units.Add(unit);
        }
    }

    /// <summary>
    /// Assigns the list when being loaded from saved data.
    /// </summary>
    /// <param name="_turns"></param>
    public void AssignList(int _turns)
    {
        for (int i = 1; i <= _turns; i++)
        {
            AddUnitsByTier(i);
        }
    }
}

