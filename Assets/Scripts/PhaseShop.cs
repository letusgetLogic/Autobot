using System;
using UnityEngine;

public class PhaseShop : MonoBehaviour
{
    public static PhaseShop Instance { get; private set; }

    [SerializeField]
    private GameObject unitPrefab;

    [Header("Reference")]
    [SerializeField]
    private GameObject[] battleSlots; 
    private Vector2[] battleSlotPos;

    [SerializeField]
    private GameObject[] shopUnitsSlots; 
    private Vector2[] shopUnitsSlotsPos;

    [SerializeField]
    private GameObject[] extraShopUnitsSlots; 
    private Vector2[] extraShopUnitsSlotsPos;

    [SerializeField]
    private GameObject[] shopItemsSlots; 
    private Vector2[] shopItemsSlotsPos;

    [SerializeField]
    private GameObject[] extraShopItemsSlots; 
    private Vector2[] extraShopItemsSlotsPos;

    [SerializeField]
    private int 
        startCoins = 10,
        rollCost = 1;

    private int coins { get; set; }


    private Template template { get; set; }

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
             Destroy(Instance.gameObject);
        }
        Instance = this;

        Initialize();
    }

    /// <summary>
    /// Initializes the positions once.
    /// </summary>
    private void Initialize()
    {
        battleSlotPos = new Vector2[battleSlots.Length];
        for (int i = 0; i < battleSlots.Length; i++)
            battleSlotPos[i] = battleSlots[i].transform.position;

        shopUnitsSlotsPos = new Vector2[shopUnitsSlots.Length];
        for (int i = 0; i < shopUnitsSlots.Length; i++)
            shopUnitsSlotsPos[i] = shopUnitsSlots[i].transform.position;

        extraShopUnitsSlotsPos = new Vector2[extraShopUnitsSlots.Length];
        for (int i = 0; i < extraShopUnitsSlots.Length; i++)
            extraShopUnitsSlotsPos[i] = extraShopUnitsSlots[i].transform.position;

        shopItemsSlotsPos = new Vector2[shopItemsSlots.Length];
        for (int i = 0; i < shopItemsSlots.Length; i++)
            shopItemsSlotsPos[i] = shopItemsSlots[i].transform.position;

        extraShopItemsSlotsPos = new Vector2[extraShopItemsSlots.Length];
        for (int i = 0; i < extraShopItemsSlots.Length; i++)
            extraShopItemsSlotsPos[i] = extraShopItemsSlots[i].transform.position;
    }

    /// <summary>
    /// Initialize template and counts turn.    
    /// </summary>
    /// <param name="_template"></param>
    public void StartTurn(Template _template)
    {
        template = _template;
        template.Turns++;
    }

    /// <summary>
    /// Roll the shop for new units and items.
    /// </summary>
    public void Roll()
    {
        if (coins < rollCost)
        {
            // hint no enough coins
            return;
        }

        coins -= rollCost;

        for (int i = 0; i < shopUnitsSlots.Length; i++)
        {
            var unitData = GameManager.Instance.Units
                [UnityEngine.Random.Range(0, GameManager.Instance.Units.Count)];

            GameObject unit = 
                Instantiate(unitPrefab, shopUnitsSlotsPos[i], Quaternion.identity);

            //unit.GetComponent<UnitController>().Initialize(unitData);
        }
    }
}
