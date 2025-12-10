using TMPro;
using UnityEngine;

public class PhaseBattleView : MonoBehaviour
{
    public static PhaseBattleView Instance { get; private set; }

    [Header("Player left")]
    [SerializeField] private TextMeshProUGUI name1;
    [SerializeField] private TextMeshProUGUI turn1, wins1, lives1;

    [Header("Player right")]
    [SerializeField] private TextMeshProUGUI name2;
    [SerializeField] private TextMeshProUGUI turn2, wins2, lives2;

    [Header("Info Label")]
    [SerializeField] private TextMeshProUGUI label;

    [Header("Speed Controller")]
    [SerializeField] private GameObject speedButton;
    [SerializeField] private TextMeshProUGUI defaultMult;
    [SerializeField] private TextMeshProUGUI maxMult;

    [Header("Running states")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject play;
    [SerializeField] private GameObject stop;


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
    public void Initialize(PlayerData _player1, PlayerData _player2)
    {
        name1.text = _player1.Name;
        turn1.text = _player1.Turn.ToString();
        wins1.text = _player1.Wins.ToString();
        lives1.text = _player1.Lives.ToString();

        name2.text = _player2.Name;
        turn2.text = _player2.Turn.ToString();
        wins2.text = _player2.Wins.ToString();
        lives2.text = _player2.Lives.ToString();

        defaultMult.text = GameManager.Instance.DefaultSpeedMultiplier.ToString();
        maxMult.text = GameManager.Instance.MaxSpeedMultiplier.ToString();
        ShowSpeedMult();
    }

    /// <summary>
    /// Updates lives of both players.
    /// </summary>
    /// <param name="pla_yer1"></param>
    /// <param name="_player2"></param>
    public void UpdateLives(PlayerData pla_yer1, PlayerData _player2)
    {
        lives1.text = pla_yer1.Lives.ToString();
        lives2.text = _player2.Lives.ToString();
    }

    /// <summary>
    /// Shows the winner.
    /// </summary>
    /// <param name="_winner"></param>
    /// <param name="_isGameOver"></param>
    public void ShowWinner(string _winner, bool _isGameOver)
    {
        label.text = $"{_winner} won this {(_isGameOver ? "tournament" : "battle")}!";
        label.enabled = true;
    }


    #region Speed Controller

    /// <summary>
    /// Sets the speed multiplier active true/false.
    /// </summary>
    /// <param name="_value"></param>
    public void SetSpeedButton(bool _value)
    {
        playButton.SetActive(_value);
        speedButton.SetActive(_value);
    }

    /// <summary>
    /// Switchs speed multipliers.
    /// </summary>
    public void SetMultiplier()
    {
        GameManager.Instance.IsDefaultMult = !GameManager.Instance.IsDefaultMult;

        GameManager.Instance.CurrentSpeedMultiplier =
            GameManager.Instance.IsDefaultMult
            ? GameManager.Instance.DefaultSpeedMultiplier
            : GameManager.Instance.MaxSpeedMultiplier;

       ShowSpeedMult();
    }

    /// <summary>
    /// Shows the speed nultiplier based on boolean.
    /// </summary>
    private void ShowSpeedMult()
    {
        defaultMult.enabled = GameManager.Instance.IsDefaultMult;
        maxMult.enabled = !GameManager.Instance.IsDefaultMult;
    }

    #endregion

    /// <summary>
    /// Button click and click on screen call.
    /// </summary>
    public void OnRunningButtonClick()
    {
        PhaseBattleController.Instance.SetRunning();
        SetRunningButton();
    }

    /// <summary>
    /// Sets the running button.
    /// </summary>
    public void SetRunningButton()
    {
        play.SetActive(!PhaseBattleController.Instance.IsStopped);
        stop.SetActive(PhaseBattleController.Instance.IsStopped);
    }

}
