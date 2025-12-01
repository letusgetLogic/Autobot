[System.Serializable]
public struct Currency
{
    /// <summary>
    /// In the actual game, Nuts are repesented for Coins.
    /// </summary>
    public int Nut;

    /// <summary>
    /// This property Tool is used to sepearate Repair System feature.
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
        set 
        { 
            SoTool = value; 
        }
    }

    /// <summary>
    /// This field Tool is setted by scriptable object, 
    /// because of losing reference of property in scriptable object.
    /// </summary>
    public int SoTool;
}
