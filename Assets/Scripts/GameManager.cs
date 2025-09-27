using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState state;
    private Template[] templates;

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
                RunGame();
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
    /// Starts game.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void RunGame()
    {
        int turns = 1;

        do
        {
            SceneManager.LoadScene("PhaseShop");
        } while (state != GameState.EndOfGame); 
    }
}   
                                                                                                                                          