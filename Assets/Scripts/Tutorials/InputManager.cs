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

    private bool isInputBlocked = true;
    private bool isChecking = false;

    /// <summary>
    /// Constructor of InputManager.
    /// </summary>
    private InputManager()
    {

    }

    public bool IsBlockingInput(InputKey _key)
    {
        if (isChecking)
            return true;

        isChecking = true;

        if (_key == InputKey.AlwaysEnabled)
        {
            isChecking = false;
            return false;
        }

        if (GameManager.Instance.IsTutorialRunning && TutorialManager.Instance)
        {
            var allowedInputs = TutorialManager.Instance.CurrentAllowedInputs;
            if (allowedInputs != null)
            {
                foreach (var allowedInput in allowedInputs)
                {
                    if (allowedInput == InputKey.All)
                    {
                        isChecking = false;
                        return isInputBlocked;
                    }

                    if (allowedInput == _key)
                    {
                        isChecking = false;
                        return false;
                    }
                }
            }

            isChecking = false;
            return true;
        }

        isChecking = false;
        return isInputBlocked;
    }
}
