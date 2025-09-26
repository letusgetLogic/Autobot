using UnityEngine;

public class PhaseShop : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField]
    private GameObject[] battleSlots; 
    private Vector2[] battleSlotPos;


    [SerializeField]
    private int startCoins = 10;
    private int coins;

    

    private void Awake()
    {
        battleSlotPos = new Vector2[battleSlots.Length];
        for (int i = 0; i < battleSlots.Length; i++)
        {
            battleSlotPos[i] = battleSlots[i].transform.position;
        }
    }
}
