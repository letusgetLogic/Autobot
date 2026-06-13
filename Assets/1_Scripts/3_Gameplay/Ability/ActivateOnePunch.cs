using System.Collections;

public class ActivateOnePunch : AbilityBase
{
    public ActivateOnePunch(UnitController _controller, Level _currentLevel, int _seed) : base(_controller, _currentLevel, _seed)
    {
    }

    protected override IEnumerator Activate()
    {
        Controller.ActivateOnePunch(true);

        yield return null;

        Coroutine = null;
    }
}