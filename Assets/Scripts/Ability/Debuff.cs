using System;
using System.Collections;
using System.Collections.Generic;

public class Debuff : AbilityBase
{
    private readonly Slot[] teamSlots;
    private readonly Slot[] enemySlots;

    /// <summary>
    /// Constructor of Debuff.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teamSlots"></param>
    public Debuff(UnitController _controller, Level _currentLevel, int _seed)
        : base(_controller, _currentLevel, _seed)
    {
        teamSlots = _controller.TeamSlots;
        enemySlots = _controller.EnemySlots;
    }

    protected override IEnumerator Activate()
    {
        var rand = new Random(RandomSeed);

        switch (CurrentLevel.ToWho)
        {
            case ToWho.None:
                UnityEngine.Debug.LogWarning($"{Controller.name} has ToWho.None!");
                break;

            case ToWho.RandomEnemy:
                DebuffRandomEnemy(rand);
                break;

                // not used yet

            //case var a when a == ToWho.NearestMateAhead:
            //    BuffNearest(GetNearest(a, CurrentLevel.ToWhoCount));
            //    break;

            //case var b when b == ToWho.NearestMateBehind:
            //    BuffNearest(GetNearest(b, CurrentLevel.ToWhoCount));
            //    break;

            //case ToWho.AllMates:
            //    BuffAllMates();
            //    break;
        }

        if (CurrentLevel.ToWho != ToWho.None)
            EventManager.Instance.OnHurt?.Invoke();

        yield return null;

        Coroutine = null;
    }

    /// <summary>
    /// Applies a debuff to a random enemy unit for a specified number of times.
    /// </summary>
    /// <param name="_rnd">The random number generator used to select enemy units.</param>
    private void DebuffRandomEnemy(Random _rnd)
    {
        List<UnitController> teams = AllBotsIn(enemySlots);

        for (int i = 0; i < CurrentLevel.ToWhoCount; i++)
        {
            if (teams.Count == 0)
                return;

            var unit = teams[_rnd.Next(0, teams.Count)];

            if (DebuffUnit(unit))
                teams.Remove(unit);
        }
    }

    /// <summary>
    /// Applies a debuff to the specified unit, dealing damage based on the current level's debuff value.
    /// </summary>
    /// <param name="_unit">The unit to apply the debuff to.</param>
    /// <returns>true if the debuff was applied; otherwise, false.</returns>
    private bool DebuffUnit(UnitController _unit)
    {
        if (_unit == null)
            return false;

        // Deal damage
        _unit.TakeDamage(new Damage(Math.Abs(CurrentLevel.Debuff.HP)));

        return true;
    }
}
