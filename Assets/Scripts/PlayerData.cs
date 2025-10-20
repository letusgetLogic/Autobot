using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string Name { get; private set; }
    public int Lives { get; set; }
    public int WinCondition { get; set; }
    public int Turns { get; set; }
    public int Wins { get; set; }
    public UnitController[] BattleUnits { get; set; }
    public UnitController[] FreezedUnits { get; set; }
    public int Coins { get; set; }

    /// <summary>
    /// Assigns the template values.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_lives"></param>
    /// <param name="_wins"></param>
    public PlayerData(string _name, int _lives, int _wins)
    {
        Lives = _lives;
        WinCondition = _wins;
        Turns = 0;
        Name = _name;
    }
}
