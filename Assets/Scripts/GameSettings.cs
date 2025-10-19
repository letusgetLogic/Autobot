using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public TMP_InputField ModeSingleHeart;

    [SerializeField]
    private int 
        maxHealth = 10,
        minHealth = 3,
        maxWins = 10,
        minWins = 3;

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
    }

    /// <summary>
    /// Start game with selected settings.  
    /// </summary>
    public void StartGame()
    {
        //switch (GameManager.Instance.Mode)
        //{
        //    case GameMode.Single:
                
        //        int b = int.Parse(ModeSingleHeart.text);
        //        if (b < minHealth || b > maxHealth) 
        //            return;
        //        GameManager.Instance.WinsCondition = a;
        //        GameManager.Instance.PlayerLives = b;
        //        GameManager.Instance.LoadGame();
                
        //        break;
        //}
    }

}

