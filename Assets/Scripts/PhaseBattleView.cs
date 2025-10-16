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
    public void Initialize(Template player1, Template player2)
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
}
