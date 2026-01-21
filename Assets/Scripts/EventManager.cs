using System;

public class EventManager
{
    #region Instance Lazy Loading
    public static EventManager Instance
    {
        get
        {
            // Lazy loading
            if (instance == null)
            {
                instance = new EventManager();
            }

            return instance;
        }
    }
    private static EventManager instance;

    private EventManager() { }

    #endregion


    #region Phase Shop

    public Action OnSettingAttachedObject { get; set; }
    public Action OnSettingNullObject { get; set; }
    
    public Action OnTransportUnit { get; set; }


    #endregion

}
