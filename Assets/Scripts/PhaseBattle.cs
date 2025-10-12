using TMPro;
using UnityEngine;

public class PhaseBattle : MonoBehaviour
{
    public static PhaseBattle Instance { get; private set; }

    [Header("Player left")]
    [SerializeField]
    private TextMeshProUGUI name1;
    [SerializeField]
    private TextMeshProUGUI turn1, wins1, lives1;
    [SerializeField]
    private Transform[] slots1;

    [Header("Player right")]
    [SerializeField]
    private TextMeshProUGUI name2;
    [SerializeField]
    private TextMeshProUGUI turn2, wins2, lives2;
    [SerializeField]
    private Transform[] slots2;

    private Template player1, player2;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    /// <summary>
    /// Initializes the players.
    /// </summary>
    /// <param name="_player1"></param>
    /// <param name="_player2"></param>
    public void Initialize(Template _player1, Template _player2)
    {
        player1 = _player1;
        name1.text = player1.Name;
        turn1.text = player1.Turns.ToString();
        wins1.text = player1.Wins.ToString();
        lives1.text = player1.Lives.ToString();
        player2 = _player2;
        name2.text = player2.Name;
        turn2.text = player2.Turns.ToString();
        wins2.text = player2.Wins.ToString();
        lives2.text = player2.Lives.ToString();

        SetUnitsToPosition();
    }

    /// <summary>
    /// Instantiates the units.
    /// </summary>
    private void SetUnitsToPosition()
    {
        SetUnitsToPosition(player1, slots1, false);
        SetUnitsToPosition(player2, slots2, true);
    }

    /// <summary>
    /// Instantiates the units.
    /// </summary>
    private void SetUnitsToPosition(Template player, Transform[] slots, bool isRight)
    {
        for (int i = 0; i < player.BattleSlots.Length; i++)
        {
            var unit = player.BattleSlots[i].Unit();
            if (unit != null)
            {
                var unitOnScene = Instantiate(unit);
                unitOnScene.transform.SetParent(slots[i].transform, false);
                if (isRight)
                    unitOnScene.GetComponent<UnitView>().SetRightSide();
            }
        }
    }
}
