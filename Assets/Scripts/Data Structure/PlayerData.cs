[System.Serializable]
public struct PlayerData
{
    public string Name { get; private set; }
    public int Lives { get; set; }
    public int Turn { get; set; }
    public int Nuts { get; set; }
    public int Tools { get; set; }

    /// <summary>
    /// Wins saves the number of wins the player has achieved so far, ONLY used in arena mode.
    /// </summary>
    public int Wins { get; set; }
   
    /// <summary>
    /// - save the data of team units from the shop phase
    /// - load them in the battle phase
    /// - override changes after the battle phase
    /// - load them back in the shop phase
    /// </summary>
    public SaveUnitData[] TeamUnitDatas {  get; set; }

    /// <summary>
    /// - save the data of the to be charged unit in the shop phase
    /// - load it in the next shop phase
    /// - give energy to the unit at the start of the shop phase
    /// </summary>
    public SaveUnitData ChargeUnitData {  get; set; }

    /// <summary>
    /// - save the data of shop bots from the shop phase
    /// - load them in the next shop phase
    /// </summary>
    public SaveUnitData[] ShopBotDatas {  get; set; }

    /// <summary>
    /// - save the data load them in the next shop phaseata of shop items from the shop phase
    /// 
    /// </summary>
    public SaveUnitData[] ShopItemDatas {  get; set; }

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
        Wins = _wins;

        Turn = default;
        Nuts = default;
        Tools = default;

        TeamUnitDatas = default;
        ChargeUnitData = default;
        ShopBotDatas = default;
        ShopItemDatas = default;
    }
}
