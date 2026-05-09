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

    [Header("Default Pack")]
    [SerializeField] private SoPack soPack;

    [Header("Prefab Settings")]
    public GameObject prefabToSpawn;
    public Sprite sprite;
    public int numberOfPrefabs = 10;

    [Header("Area Settings")]
    public Vector2 areaMin = new Vector2(-10, -10); // Min X and Y
    public Vector2 areaMax = new Vector2(10, 10);   // Max X and Y

    [Header("Rotation Settings")]
    public float rotationRangeMin = -90f; // Degrees for random rotation
    public float rotationRangeMax = 90f; // Degrees for random rotation

    [ContextMenu("Create lot of robots")]
    void SpawnPrefabsInArea()
    {
        for (int i = 0; i < numberOfPrefabs; i++)
        {
            // 1. Generate Random Position within the area
            float randomX = Random.Range(areaMin.x, areaMax.x);
            float randomY = Random.Range(areaMin.y, areaMax.y);
            Vector3 spawnPosition = new Vector3(randomX, randomY, transform.position.z);

            // 2. Generate Random Rotation (Direction)
            // For 2D, rotate around Z axis. For 3D, use random Euler angles.
            float randomAngle = Random.Range(rotationRangeMin, rotationRangeMax);
            Quaternion spawnRotation = Quaternion.Euler(0, 0, randomAngle);

            // 3. Instantiate the prefab
            var go = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

            // 4. Set visual
            var bots = soPack.BotsTier1;
            int rand = Random.Range(0, bots.Length);
            var unit = bots[rand];
            go.GetComponent<SpriteRenderer>().sprite = unit.Sprite;

            go.name = DebugID++ + "_" + unit.name;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

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
    /// <param name="_turn"></param>
    public void AssignList(int _turn)
    {
        for (int i = 1; i <= _turn; i++)
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
    private void AddUnitsByTier(int _turn)
    {
        (bool isUnlocking, int tier) = IsUnlockingTier(_turn);

        if (isUnlocking)
        {
            AddUnits(MyPack.Bots[tier - 1], MyPack.Items[tier - 1]);
        }
    }

    public (bool, int) IsUnlockingTier(int _turn)
    {
        int a = MyPack.Tier1AvaiableAtTurn.Value;
        int b = MyPack.Tier2AvaiableAtTurn.Value;
        int c = MyPack.Tier3AvaiableAtTurn.Value;
        int d = MyPack.Tier4AvaiableAtTurn.Value;
        int e = MyPack.Tier5AvaiableAtTurn.Value;
        int f = MyPack.Tier6AvaiableAtTurn.Value;

        if (_turn == a && MyPack.BotsTier1 != null && MyPack.BotsTier1.Length > 0)
            return (true, 1);

        if (_turn == b && MyPack.BotsTier2 != null && MyPack.BotsTier2.Length > 0)
            return (true, 2);

        if (_turn == c && MyPack.BotsTier3 != null && MyPack.BotsTier3.Length > 0)
            return (true, 3);

        if (_turn == d && MyPack.BotsTier4 != null && MyPack.BotsTier4.Length > 0)
            return (true, 4);

        if (_turn == e && MyPack.BotsTier5 != null && MyPack.BotsTier5.Length > 0)
            return (true, 5);

        if (_turn == f && MyPack.BotsTier6 != null && MyPack.BotsTier6.Length > 0)
            return (true, 6);

        return (false, 0);
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
            bool alreadyHasUnit = false;
            foreach (var item in Bots)
            {
                if (item == unit)
                {
                    alreadyHasUnit = true;
                    break;
                }
            }
            if (alreadyHasUnit == false)
                Bots.Add(unit);
        }
        foreach (var unit in _itemTier)
        {
            bool alreadyHasUnit = false;
            foreach (var item in Items)
            {
                if (item == unit)
                {
                    alreadyHasUnit = true;
                    break;
                }
            }
            if (alreadyHasUnit == false)
                Items.Add(unit);
        }
    }

    /// <summary>
    /// Return scriptable object with index or ID.
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    public SoUnit GetSoUnit(SaveUnitData _data)
    {
        if (_data.UnitType == UnitType.SummonedRobot)
        {
            foreach (var bot in MyPack.SummonedBots)
            {
                if (bot.ID == _data.Index)
                    return bot;
            }
        }
        if (_data.UnitType == UnitType.Item)
        {
            foreach (var item in MyPack.TemporaryItems)
            {
                if (item.ID == _data.Index)
                    return item;
            }
        }

        return Bots[_data.Index];
    }

    public void ResetPack()
    {
        Bots.Clear();
        Items.Clear();
    }
}

