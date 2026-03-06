using System.Collections;

public class ConvertEnergy : AbilityBase
{
    /// <summary>
    /// Initializes a new instance of the ConvertEnergy class with the specified controller, level, and seed.
    /// </summary>
    /// <param name="_controller">The unit controller responsible for managing unit operations.</param>
    /// <param name="_currentLevel">The current level context.</param>
    /// <param name="_seed">The seed value for randomization.</param>
    public ConvertEnergy(UnitController _controller, Level _currentLevel, int _seed) 
        : base(_controller, _currentLevel, _seed)
    {
    }

    protected override IEnumerator Activate()
    {
        int energy = Controller.Model.Data.Cur.ENG;
        if (energy < 0)
            yield break;

        // Execute only when the current energy > 0

        Controller.SetEnergy(-energy, false);

        Attribute buff = default;

        // If the buff is HP, convert all energy to HP, otherwise convert all energy to ATK
        if (CurrentLevel.Buff.HP > 0)
            buff = new Attribute(energy, 0);
        else
            if (CurrentLevel.Buff.ATK > 0)
            buff = new Attribute(0, energy);

        Controller.Buff(IsPernament(CurrentLevel.AbilityDuration), buff);

        if (CurrentLevel.ToWho != ToWho.None)
            EventManager.Instance.OnBuff?.Invoke();

        yield return null;

        Coroutine = null;
    }
}
