using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseBattleView : MonoBehaviour
{
    public static PhaseBattleView Instance { get; private set; }

    [Header("Player left")]
    [SerializeField] private TextMeshProUGUI name1;
    [SerializeField] private TextMeshProUGUI turn1, wins1, lives1;

    [Header("Player right")]
    [SerializeField] private TextMeshProUGUI name2;
    [SerializeField] private TextMeshProUGUI turn2, wins2, lives2;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI clickText;

    // This code block or the time scaling feature is disabled, because it cause inaccuracy, because the time from start coroutine wasn't scaled too.
    //[Header("Speed Controller")]
    //[SerializeField] private GameObject speedButton;
    //[SerializeField] private TextMeshProUGUI defaultMult;
    //[SerializeField] private TextMeshProUGUI maxMult;

    [Header("Running states")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject play;
    [SerializeField] private GameObject stop;

    [Header("Visuals")]
    [SerializeField] private Image collideVisual;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        collideVisual.enabled = false;
    }

    /// <summary>
    /// Initializes the players.
    /// </summary>
    /// <param name="_player1"></param>
    /// <param name="_player2"></param>
    public void Initialize(bool defaultName, PlayerData _player1, PlayerData _player2)
    {
        name1.text = defaultName ? "You" : _player1.Name;
        if (turn1) turn1.text = _player1.Turn.ToString();
        if (wins1) wins1.text = _player1.Wins.ToString();
        if (lives1) lives1.text = _player1.Lives.ToString();

        name2.text = _player2.Name;
        if (turn2) turn2.text = _player2.Turn.ToString();
        if (wins2) wins2.text = _player2.Wins.ToString();
        if (lives2) lives2.text = _player2.Lives.ToString();

        // This code block or the time scaling feature is disabled, because it cause inaccuracy, because the time from start coroutine wasn't scaled too.
        //defaultMult.text = GameManager.Instance.DefaultSpeedMultiplier.ToString();
        //maxMult.text = GameManager.Instance.MaxSpeedMultiplier.ToString();
        //ShowSpeedMult();
    }

    /// <summary>
    /// Updates lives of both players.
    /// </summary>
    /// <param name="_player1"></param>
    /// <param name="_player2"></param>
    public void UpdateLives(int _player1, int _player2)
    {
        if (lives1) lives1.text = _player1.ToString();
        if (lives2) lives2.text = _player2.ToString();
    }

    /// <summary>
    /// Shows the winner.
    /// </summary>
    /// <param name="_winner"></param>
    /// <param name="_isGameOver"></param>
    public void ShowWinner(string _winner, bool _isGameOver)
    {
        playButton.SetActive(false);
        label.text = $"{_winner} won this {(_isGameOver ? "game" : "battle")}!";
        label.enabled = true;
    }


    #region Speed Controller - This code block or the time scaling feature is disabled, because it cause inaccuracy, because the time from start coroutine wasn't scaled too.

    ///// <summary>
    ///// Sets the speed multiplier active true/false.
    ///// </summary>
    ///// <param name="_value"></param>
    //public void SetSpeedButton(bool _value)
    //{
    //    playButton.SetActive(_value);
    //    speedButton.SetActive(_value);
    //}

    ///// <summary>
    ///// Switchs speed multipliers.
    ///// </summary>
    //public void SetMultiplier()
    //{
    //    GameManager.Instance.IsDefaultMult = !GameManager.Instance.IsDefaultMult;

    //    GameManager.Instance.CurrentSpeedMultiplier =
    //        GameManager.Instance.IsDefaultMult
    //        ? GameManager.Instance.DefaultSpeedMultiplier
    //        : GameManager.Instance.MaxSpeedMultiplier;

    //   ShowSpeedMult();
    //}

    ///// <summary>
    ///// Shows the speed nultiplier based on boolean.
    ///// </summary>
    //private void ShowSpeedMult()
    //{
    //    defaultMult.enabled = GameManager.Instance.IsDefaultMult;
    //    maxMult.enabled = !GameManager.Instance.IsDefaultMult;
    //}

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

    /// <summary>
    /// Shows the collide.
    /// </summary>
    public void ShowCollideVisual()
    {
        collideVisual.enabled = true;
    }

    /// <summary>
    /// Hides the collide visual after duration.
    /// </summary>
    /// <returns></returns>
    public IEnumerator HideCollideVisual(float _duration)
    {
        yield return new WaitForSeconds(_duration);

        collideVisual.enabled = false;
    }

    /// <summary>
    /// Shows the hint to click.
    /// </summary>
    public IEnumerator ShowClick(float _duration)
    {
        yield return new WaitForSeconds(_duration);

        clickText.gameObject.SetActive(true);
    }
}
