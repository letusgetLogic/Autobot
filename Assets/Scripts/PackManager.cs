using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PackManager : MonoBehaviour
{
    public static PackManager Instance { get; private set; }

    public SoPack MyPack { get; private set; }

    public List<SoUnit> Bots { get; private set; } = new List<SoUnit>();
    public List<SoUnit> Items { get; private set; } = new List<SoUnit>();

    public int DebugID { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
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
        if (MyPack != null)
            MyPack = null;

        MyPack = _selectedPack;

        //AddBots(MyPack.BotsTier1);
        //AddItems(MyPack.ItemsTier1);

        //AddBots(MyPack.BotsTier2);
        //AddItems(MyPack.ItemsTier2);

        //AddBots(MyPack.BotsTier3);
        //AddItems(MyPack.ItemsTier3);

        //AddBots(MyPack.BotsTier4);
        //AddItems(MyPack.ItemsTier4);

        //AddBots(MyPack.BotsTier5);
        //AddItems(MyPack.ItemsTier5);

        //AddBots(MyPack.BotsTier6);
        //AddItems(MyPack.ItemsTier6);
    }

    /// <summary>
    /// Adds the robots to the list.
    /// </summary>
    /// <param name="_units"></param>
    private void AddBots(SoUnit[] _units)
    {
        if (_units != null)
        {
            for (int i = 0; i < _units.Length; i++)
            {
                Bots.Add(_units[i]);
            }
        }
    }

    /// <summary>
    /// Adds the items to the list.
    /// </summary>
    /// <param name="_units"></param>
    private void AddItems(SoUnit[] _units)
    {
        if (_units != null)
        {
            for (int i = 0; i < _units.Length; i++)
            {
                Items.Add(_units[i]);
            }
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

    /// <summary>
    /// Adds units to the collection based on the specified turn and their availability tier.
    /// </summary>
    /// <remarks>This method checks the availability of units for each tier based on predefined turn
    /// thresholds. If the specified turn matches the availability turn for a particular tier, the corresponding units
    /// are added. If no match is found, no units are added.</remarks>
    /// <param name="_turns">The current turn number. Determines which tier of units will be added.</param>
    private void AddUnitsByTier(int _turns)
    {
        int a = MyPack.Tier1AvaiableAtTurn.Value;
        int b = MyPack.Tier2AvaiableAtTurn.Value;
        int c = MyPack.Tier3AvaiableAtTurn.Value;
        int d = MyPack.Tier4AvaiableAtTurn.Value;
        int e = MyPack.Tier5AvaiableAtTurn.Value;
        int f = MyPack.Tier6AvaiableAtTurn.Value;

        if (_turns == a)
        {
            AddUnits(MyPack.BotsTier1, MyPack.ItemsTier1);
        }
        else if (_turns == b)
        {
            AddUnits(MyPack.BotsTier2, MyPack.ItemsTier2);
        }
        else if (_turns == c)
        {
            AddUnits(MyPack.BotsTier3, MyPack.ItemsTier3);
        }
        else if (_turns == d)
        {
            AddUnits(MyPack.BotsTier4, MyPack.ItemsTier4);
        }
        else if (_turns == e)
        {
            AddUnits(MyPack.BotsTier5, MyPack.ItemsTier5);
        }
        else if (_turns == f)
        {
            AddUnits(MyPack.BotsTier6, MyPack.ItemsTier6);
        }
    }

    /// <summary>
    /// Adds the specified units to the game's unit collection.
    /// </summary>
    /// <remarks>This method adds all units from the provided array to the global unit collection managed by
    /// the game. Ensure that the array is not null and contains valid units before calling this method.</remarks>
    /// <param name="_botTier">An array of bots to be added. Each bot in the array will be added to the game's bot collection.</param>
    /// <param name="_itemTier">An array of items to be added. Each item in the array will be added to the game's item collection.</param>
    private void AddUnits(SoUnit[] _botTier, SoUnit[] _itemTier)
    {
        foreach (var unit in _botTier)
        {
            Bots.Add(unit);
        }
        foreach (var unit in _itemTier)
        {
            Items.Add(unit);
        }
    }

    /// <summary>
    /// Return scriptable object with index or ID.
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    public (SoUnit soUnit, int index) GetSoUnit(SaveUnitData _data)
    {
        if (_data.UnitType == UnitType.SummonedRobot)
        {
            foreach (var bot in MyPack.SummonedBots)
            {
                if (bot.ID == _data.Index)
                    return (bot, _data.Index);
            }
        }

        return (Bots[_data.Index], _data.Index);
    }
}

