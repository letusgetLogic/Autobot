using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhaseBattleView : MonoBehaviour
{
    public static PhaseBattleView Instance { get; private set; }

    [Header("Buttons")]
    [SerializeField] private List<GameObject> buttons;
    [SerializeField] private GameObject replayButton;

    [Header("Panel")]
    [SerializeField] private LerpMovement bottomPanel;

    [Header("Player left")]
    [SerializeField] private TextMeshProUGUI name1;
    [SerializeField] private TextMeshProUGUI turn1, wins1, lives1;

    [Header("Player right")]
    [SerializeField] private TextMeshProUGUI name2;
    [SerializeField] private TextMeshProUGUI turn2, wins2, lives2;

    [Header("End Screen")]
    [SerializeField] private LightenUpDown coverPanel;
    [SerializeField] private GameObject clickText;
    [SerializeField] private TextMeshProUGUI labelWinner;
    [SerializeField] private TextMeshProUGUI labelContent;
    [SerializeField] private GameObject winnerPanel;
    public GameObject WinnerPanel => winnerPanel;

    [Header("Canvases")]
    [SerializeField] private GameObject canvas1;
    public GameObject Canvas1 => canvas1;

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
        labelWinner.enabled = false;
        labelContent.enabled = false;
        winnerPanel.SetActive(false);
        ShowPanelText(false);
        replayButton.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnSettingsButtonOpen += HideWinnerPanel;
        EventManager.Instance.OnSettingsButtonClose += ShowWinnerPanel;
        EventManager.Instance.OnReplayButtonClick += HideWinnerPanel;
    }

    private void OnDisable()
    {
        GameManager.Instance.IsGameOverSceneRunning = false;

        EventManager.Instance.OnSettingsButtonOpen -= HideWinnerPanel;
        EventManager.Instance.OnSettingsButtonClose -= ShowWinnerPanel;
        EventManager.Instance.OnReplayButtonClick -= HideWinnerPanel;
    }

    private void ShowWinnerPanel()
    {
        if (GameManager.Instance.CurrentGame.State == GameState.WaitingEndOfGame)
            winnerPanel.SetActive(true);
    }
    private void HideWinnerPanel() => winnerPanel.SetActive(false);

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// Initializes the players.
    /// </summary>
    /// <param name="_player1"></param>
    /// <param name="_player2"></param>
    public void Initialize(PlayerData _player1, PlayerData _player2)
    {
        name1.text = _player1.Name;
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

    public void OnOpenSceneEnd()
    {
        coverPanel.gameObject.SetActive(false);
        bottomPanel.Trigger();
        bottomPanel.OnPosition += ShowText;
    }

    private void ShowText()
    {
        ShowPanelText(true);
        bottomPanel.OnPosition -= ShowText;
    }

    private void ShowPanelText(bool _value)
    {
        name1.enabled = _value;
        if (turn1) turn1.enabled = _value;
        if (wins1) wins1.enabled = _value;
        if (lives1) lives1.enabled = _value;

        name2.enabled = _value;
        if (turn2) turn2.enabled = _value;
        if (wins2) wins2.enabled = _value;
        if (lives2) lives2.enabled = _value;
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

    public IEnumerator ShowWinnerAtEndOfBattle(bool _isDraw, string _winner)
    {
        coverPanel.gameObject.SetActive(true);
        playButton.SetActive(false);
        buttons.ForEach(b => b.SetActive(false));

        float animTime = coverPanel.SwitchOn(true);
        yield return new WaitForSeconds(animTime);

        buttons.ForEach(b => b.SetActive(true));
        EventManager.Instance.OnGameOverSound?.Invoke();

        if (_isDraw)
        {
            labelWinner.enabled = false;
            labelContent.text = "Draw!";
            labelContent.enabled = true;
            yield break;
        }

        labelWinner.enabled = false;
        labelContent.text = $"{_winner} won!";
        labelContent.enabled = true;
        yield break;

    }

    public IEnumerator ShowWinnerAtEndOfGame(string _winner, PlayerData _winnerData, UnityAction actionEndOfGame)
    {
        GameManager.Instance.IsGameOverSceneRunning = true;

        coverPanel.gameObject.SetActive(true);
        playButton.SetActive(false);
        buttons.ForEach(b => b.SetActive(false));

        float animTime = coverPanel.SwitchOn(true);
        yield return new WaitForSeconds(animTime);

        var game = GameManager.Instance.CurrentGame;
        if (game != null)
        {
            if (game.Mode == GameMode.Tutorial)
            {
                labelWinner.enabled = false;
                labelContent.text = $"{_winner} won the game!";
                labelContent.enabled = true;
            }
            else
            {
                string[] congrats = new string[]
                    {
                $" won the game!",
                $" is the best!",
                $" is unbeatable!",
                $" has the strongest team!",
                $" is the Scientist of Robotics!",
                };
                int index = new System.Random().Next(congrats.Length);

                labelWinner.text = _winner;
                labelWinner.enabled = true;
                labelContent.text = congrats[index];
                labelContent.enabled = true;
            }
        }

        WinnerPanel.SetActive(true);
        ShowWinnerTeam(_winnerData);
        EventManager.Instance.OnPopUpSound?.Invoke();

        yield return new WaitForSeconds(0.5f);

        buttons.ForEach(b => b.SetActive(true));
        replayButton.SetActive(true);
        GameManager.Instance.IsGameOverSceneRunning = false;
        actionEndOfGame?.Invoke();
    }

    private void ShowWinnerTeam(PlayerData _data)
    {
        var init = new InitializeState(0f);
        var team = init.SpawnUnitsByData(_data, PhaseBattleController.Instance.WinnerSlots, true);

        foreach (var unit in team)
        {
            if (unit != null)
            {
                unit.transform.SetParent(WinnerPanel.transform, true);
                unit.View.ShowByGameOver();
            }
        }

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
        if (InputManager.Instance.IsBlockingInput(InputKey.ClickButtonPlayInBattle))
            return;

        PhaseBattleController.Instance.SetRunning(GameManager.Instance.IsStopped, true);
    }

    /// <summary>
    /// Sets the running button.
    /// </summary>
    public void SetRunningButton()
    {
        play.SetActive(!GameManager.Instance.IsStopped);
        stop.SetActive(GameManager.Instance.IsStopped);
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

        clickText.SetActive(true);
    }
}
