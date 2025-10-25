using System.Collections;
using TMPro;
using UnityEngine;

public class PhaseBattleView : MonoBehaviour
{
    public static PhaseBattleView Instance { get; private set; }

    [Header("Player left")]
    [SerializeField]
    private TextMeshProUGUI name1;
    [SerializeField]
    private TextMeshProUGUI turn1, wins1, lives1;

    [Header("Player right")]
    [SerializeField]
    private TextMeshProUGUI name2;
    [SerializeField]
    private TextMeshProUGUI turn2, wins2, lives2;

    [Header("Info Label")]
    [SerializeField]
    private TextMeshProUGUI label;

    [SerializeField]
    private GameObject
        speedButton,
        defaultMult,
        maxMult;


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
    public void Initialize(PlayerData player1, PlayerData player2)
    {
        name1.text = player1.Name;
        turn1.text = player1.Turns.ToString();
        wins1.text = player1.Wins.ToString();
        lives1.text = player1.Lives.ToString();

        name2.text = player2.Name;
        turn2.text = player2.Turns.ToString();
        wins2.text = player2.Wins.ToString();
        lives2.text = player2.Lives.ToString();
    }

    public void ShowWinner(string winner, bool isGameOver)
    {
        label.text = $"{winner} won this {(isGameOver ? "duel" : "battle")}!";
        label.enabled = true;
    }

    private bool isDefaultMult = true;

    /// <summary>
    /// Sets the speed multiplier active true/false.
    /// </summary>
    /// <param name="activ"></param>
    public void SetSpeedButton(bool activ)
    {
        speedButton.SetActive(activ);
    }

    /// <summary>
    /// Switchs speed multipliers.
    /// </summary>
    public void SetMultiplier()
    {
        isDefaultMult = !isDefaultMult;

        var instance = PhaseBattleController.Instance;
        instance.SpeedMultiplier =
            isDefaultMult
            ? 1f
            : instance.MaxMultiplier;

        defaultMult.SetActive(isDefaultMult);
        maxMult.SetActive(!isDefaultMult);
    }

}
