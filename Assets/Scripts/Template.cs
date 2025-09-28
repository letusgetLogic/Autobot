using UnityEngine;

public class Template
{
    public int Health { get; set; }
    public int WinsCondition { get; set; }
    public int Turns { get; set; }
    public int Wins { get; set; }
    public GameObject[] BattleUnits { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Template"/> class with the specified health and win game condition.
    /// </summary>
    /// <param name="_health">The initial health value for the instance. Must be a non-negative integer.</param>
    /// <param name="_wins">The initial number of wins for the instance. Must be a non-negative integer.</param>
    public Template(int _health, int _wins)
    {
        Health = _health;
        WinsCondition = _wins;
        Turns = 0; 
    }   

}
