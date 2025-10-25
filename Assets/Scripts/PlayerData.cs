using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Name { get; private set; }
    public int Lives { get; set; }
    public int WinCondition { get; set; }
    public int Turns { get; set; }
    public int Wins { get; set; }
    public int Coins { get; set; }
    public UnitModel[] BattleUnitModels {  get; set; }
    public UnitModel[] ShopUnitModels {  get; set; }

    /// <summary>
    /// Assigns the template values.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_lives"></param>
    /// <param name="_wins"></param>
    public PlayerData(string _name, int _lives, int _wins)
    {
        Name = _name;
        Lives = _lives;
        WinCondition = _wins;
        Turns = 0;
    }
}
