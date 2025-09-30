using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState state;
    private Template[] templates;

    [HideInInspector] 
    public List<SoUnit> Units = new();
    [HideInInspector] 
    public List<SoItem> Items = new();

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    /// <summary>
    /// Loads game.
    /// </summary>
    public void LoadGame()
    {
        state = GameState.LoadGame;

        switch (GameSettings.Instance.Mode)
        {
            case GameMode.None:
                GameSettings.Instance.Mode = GameMode.Single;
                LoadGame();
                break;

            case GameMode.Single:
                templates = new[]
                    {
                    new Template(GameSettings.Instance.PlayerHealth, GameSettings.Instance.Wins),
                    new Template(GameSettings.Instance.PlayerHealth, GameSettings.Instance.Wins)
                    };
                RunSingle();
                break;

            case GameMode.AI:
                // Load AI game mode
                break;
            case GameMode.Versus:
                // Load Versus game mode
                break;
            default:
                Debug.LogError("Invalid game mode selected.");
                break;
        }
    }

    /// <summary>
    /// Starts game mode single.
    /// </summary>
    private void RunSingle()
    {
       foreach (var template in templates)
       {
            state = GameState.StartOfTurn;
            SceneManager.LoadScene("PhaseShop");
            StartCoroutine(StartTurn(template));
       }


    }

    /// <summary>
    /// Waits until the <see cref="PhaseShop.Instance"/> is initialized and then transitions the game state to the start
    /// of the turn.
    /// </summary>
    private IEnumerator StartTurn(Template _template)
    {
        yield return new WaitUntil(() => PhaseShop.Instance != null);
        state = GameState.StartOfTurn;
        _template.RollShopAtStart();

    }
    public void EndGame()
    {
        state = GameState.EndOfGame;
        Units.Clear();
        Items.Clear();
    }
}   
                                                                                                                                          