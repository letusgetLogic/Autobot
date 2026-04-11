
[System.Serializable]
public class PlayerData
{
    public bool IsAI { get; set; }
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
    public SaveUnitData[] TeamUnitDatas { get; set; }

    /// <summary>
    /// - save the data of the to be charged unit in the shop phase
    /// - load it in the next shop phase
    /// - give energy to the unit at the start of the shop phase
    /// </summary>
    public SaveUnitData ChargeUnitData { get; set; }

    /// <summary>
    /// - save the data of shop bots from the shop phase
    /// - load them in the next shop phase
    /// </summary>
    public SaveUnitData[] ShopBotDatas { get; set; }

    /// <summary>
    /// - save the data load them in the next shop phaseata of shop items from the shop phase
    /// 
    /// </summary>
    public SaveUnitData[] ShopItemDatas { get; set; }

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
    }

    /// <summary>
    /// Assigns the template values.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_lives"></param>
    /// <param name="_wins"></param>
    public PlayerData(string _name, int _lives, int _wins, bool _isAI)
    {
        Name = _name;
        Lives = _lives;
        Wins = _wins;
        IsAI = _isAI;
    }

    /// <summary>
    /// Makes a copy of the reference.
    /// </summary>
    /// <param name="_other"></param>
    public PlayerData(PlayerData _other)
    {
        Name = _other.Name;
        Lives = _other.Lives;
        Turn = _other.Turn;
        Nuts = _other.Nuts;
        Tools = _other.Tools;
        Wins = _other.Wins;

        if (_other.TeamUnitDatas != null)
        {
            TeamUnitDatas = new SaveUnitData[_other.TeamUnitDatas.Length];
            for (int i = 0; i < _other.TeamUnitDatas.Length; i++)
            {
                var unit = _other.TeamUnitDatas[i];
                if (unit != null)
                {
                    TeamUnitDatas[i] = new SaveUnitData(unit);
                }
            }
        }

        ChargeUnitData = _other.ChargeUnitData != null
            ? new SaveUnitData(_other.ChargeUnitData) : null;

        if (_other.ShopBotDatas != null)
        {
            ShopBotDatas = new SaveUnitData[_other.ShopBotDatas.Length];
            for (int i = 0; i < _other.ShopBotDatas.Length; i++)
            {
                var unit = _other.ShopBotDatas[i];
                if (unit != null)
                {
                    ShopBotDatas[i] = new SaveUnitData(unit);
                }
            }
        }

        if (_other.ShopItemDatas != null)
        {
            ShopItemDatas = new SaveUnitData[_other.ShopItemDatas.Length];
            for (int i = 0; i < _other.ShopItemDatas.Length; i++)
            {
                var unit = _other.ShopItemDatas[i];
                if (unit != null)
                {
                    ShopItemDatas[i] = new SaveUnitData(unit);
                }
            }
        }
    }
}
