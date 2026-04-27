using System;
using System.Collections;
using System.Collections.Generic;

public class Buff : AbilityBase
{
    /// <summary>
    /// Constructor of Buff.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_seed"></param>
    public Buff(UnitController _controller, Level _currentLevel, int _seed)
        : base(_controller, _currentLevel, _seed)
    {
    }

    protected override IEnumerator Activate()
    {
        Random rand = new Random(RandomSeed); 

        switch (CurrentLevel.ToWho)
        {
            case ToWho.None:
                UnityEngine.Debug.LogWarning($"{Controller.name} has ToWho.None!");
                break;

            case ToWho.Self:
                BuffUnit(Controller);
                break;

            case ToWho.RandomMate:
                BuffRandomMate(rand);
                break;

            case ToWho.TargetBot:
                BuffTargetByItem();
                break;

            case var a when a == ToWho.NearestMateAhead:
                NearestUnits(a, CurrentLevel.ToWhoCount).ForEach(x => BuffUnit(x));
                break;

            case var b when b == ToWho.NearestMateBehind:
                NearestUnits(b, CurrentLevel.ToWhoCount).ForEach(x => BuffUnit(x));
                break;

            case ToWho.AllMates:
                Controller.AllMates.ForEach(x => BuffUnit(x));
                break;
        }

        if (CurrentLevel.ToWho != ToWho.None)
            EventManager.Instance.OnBuff?.Invoke();

        yield return null;

        Coroutine = null;
    }

    /// <summary>
    /// Applies a buff to a random selection of allied units based on the current level's target count.
    /// </summary>
    /// <param name="_rnd">The random number generator used to select units.</param>
    private void BuffRandomMate(Random _rnd)
    {
        var mates = Controller.AllMates;

        for (int i = 0; i < CurrentLevel.ToWhoCount && mates.Count > 0; i++)
        {
            var unit = mates[_rnd.Next(0, mates.Count)];

            if (BuffUnit(unit))
                mates.Remove(unit);
        }
    }

    /// <summary>
    /// Applies a buff to the next unit in the target queue if any are available.
    /// </summary>
    private void BuffTargetByItem()
    {
        if (Targets.Count <= 0)
            return;

        var unit = Targets.Dequeue();
        BuffUnit(unit);
    }

    /// <summary>
    /// Applies a buff to the specified unit and sets it as a temporary item if applicable.
    /// </summary>
    /// <param name="_unit">The unit to buff.</param>
    /// <returns>true if the buff was applied; otherwise, false.</returns>
    private bool BuffUnit(UnitController _unit)
    {
        if (_unit == null)
            return false;

        _unit.Buff(
            IsPernament(CurrentLevel.AbilityDuration),
            CurrentLevel.Buff);

        if (Controller.Model.Data.UnitType == UnitType.Item &&
            IsPernament(CurrentLevel.AbilityDuration) == false)
            {
            _unit.View.SetTemporaryItem(true);
        }

        return true;
    }
}
