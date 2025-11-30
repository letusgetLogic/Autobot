[System.Serializable]
public class PlayerData
{
    public string Name { get; private set; }
    public int Lives { get; set; }
    public int Turns { get; set; }
    public int Nuts { get; set; }
    public int Tools { get; set; }

    // WinCondition defines how many wins are needed to win the game,
    // ONLY used in arena mode, where the number of players is not defined.
    public int WinCondition { get; set; }

    // Wins saves the number of wins the player has achieved so far, ONLY used in arena mode.
    public int Wins { get; set; }

    // TeamUnitDatas is used to
    // - save the data of team units from the shop phase
    // - load them in the battle phase
    // - override changes after the battle phase
    // - load them back in the shop phase
    public SaveUnitData[] TeamUnitDatas {  get; set; }

    // ChargeUnitData is used to
    // - save the data of the to be charged unit in the shop phase
    // - load it in the next shop phase
    // - give energy to the unit at the start of the shop phase
    public SaveUnitData ChargeUnitData {  get; set; }

    // ShopUnitDatas is used to
    // - save the data of shop units from the shop phase
    // - load them in the next shop phase
    public SaveUnitData[] ShopUnitDatas {  get; set; }

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
    }
}
