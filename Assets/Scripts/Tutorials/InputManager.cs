public class InputManager
{
    #region Instance Lazy Loading
    public static InputManager Instance
    {
        get
        {
            // Lazy loading
            if (_Instance == null)
            {
                _Instance = new InputManager();
            }

            return _Instance;
        }
    }
    private static InputManager _Instance;
    #endregion

    public bool BlocksInput { set { isInputBlocked = value; } }

    private bool isInputBlocked;

    /// <summary>
    /// Constructor of InputManager.
    /// </summary>
    private InputManager()
    {

    }

    public bool IsBlockingInput(InputKey _key)
    {
        if (_key == InputKey.AlwaysEnabled)
        {
            return false;
        }

        if (TutorialManager.Instance)
        {
            var allowedInputs = TutorialManager.Instance.CurrentAllowedInputs;
            if (allowedInputs != null)
            {
                foreach (var allowedInput in allowedInputs)
                {
                    if (allowedInput == _key)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return isInputBlocked;
    }
}
