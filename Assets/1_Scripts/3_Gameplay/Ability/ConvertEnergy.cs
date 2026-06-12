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

        int buffValue = CurrentLevel.Index switch
        {
            0 => (int)(energy * 0.5f),
            1 => energy,
            2 => energy * 2,
            _ => 0
        };

        int consumENG = CurrentLevel.Index switch
        {
            0 => buffValue * 2,
            1 => energy,
            2 => energy,
            _ => 0
        };

        Controller.AddEnergy(-consumENG, false, true);

        Attribute buff = default;
        

        // If the buff is HP, convert all energy to HP, otherwise convert all energy to ATK
        if (CurrentLevel.Buff.HP > 0)
            buff = new Attribute(buffValue, 0);
        else
            if (CurrentLevel.Buff.ATK > 0)
            buff = new Attribute(0, buffValue);

        Controller.Buff(IsPernament(CurrentLevel.AbilityDuration), buff);

        if (CurrentLevel.ToWho != ToWho.None)
            EventManager.Instance.OnBuff?.Invoke();

        yield return null;

        Coroutine = null;
    }
}
