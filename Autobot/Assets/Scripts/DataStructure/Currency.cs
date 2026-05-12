[System.Serializable]
public struct Currency
{
    /// <summary>
    /// In the actual game, Nuts are repesented for Coins.
    /// </summary>
    public int Nut;

    /// <summary>
    /// This property Tool is used to sepearate Repair System feature,
    /// because of losing reference of property in scriptable object.
    /// </summary>
    public int Tool
    {
        get
        {
            // If Repair System is enabled, tools are not consumed.

            if (GameManager.Instance != null &&
                GameManager.Instance.IsRepairSystemActive == false)
            {
                return 0;
            }

            return SoTool;
        }
    }

    /// <summary>
    /// This field Tool is setted by scriptable object, 
    /// because of losing reference of property in scriptable object.
    /// </summary>
    public int SoTool;

    /// <summary>
    /// Initializes a new instance of the Currency class with the specified nut and tool values.
    /// </summary>
    /// <param name="_nut">The value to assign to Nut.</param>
    /// <param name="_tool">The value to assign to SoTool.</param>
    public Currency(int _nut, int _tool)
    {
        Nut = _nut;
        SoTool = _tool;
    }
}
