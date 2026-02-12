public class TriggerAbilityManager
{
    #region Instance 
    public static TriggerAbilityManager Instance
    {
        get
        {
            // Lazy loading
            if (instance == null)
                instance = new TriggerAbilityManager();

            return instance;
        }
    }
    private static TriggerAbilityManager instance;

    private TriggerAbilityManager() { }

    #endregion

    #region Trigger Queues


    #endregion

    /// <summary>
    /// Check and trigger abilities in the right order.
    /// </summary>
    /// <param name="_unit1"></param>
    /// <param name="_unit2"></param>
    /// <returns></returns>
    public int TriggerBeforeAttack(UnitController _unit1, UnitController _unit2)
    {
        AbilityBase ability1 = _unit1.TriggerBeforeAttack(_unit2);
        AbilityBase ability2 = _unit2.TriggerBeforeAttack(_unit1);

        if (ability2 != null)
        {
            if (ability1 != null)
            {
                DoType doType1 = _unit1.Model.CurrentLevel.DoType;
                DoType doType2 = _unit2.Model.CurrentLevel.DoType;

                if ((int)doType2 > (int)doType1)
                {
                    if (doType2 == DoType.Steal)
                    {
                        EventManager.Instance.OnTriggerAbility?.Invoke(ability2, false);

                        // Check if the target unit has enough energy for trigger ability.

                        int targetENG = _unit1.Model.Data.Cur.ENG;
                        int consumENG = _unit1.Model.CurrentLevel.ConsumedEnergy.Value;
                        int stolenENG = Steal.StolenEnergy(_unit1, _unit2);

                        if (targetENG + stolenENG >= consumENG)
                        {
                            EventManager.Instance.OnTriggerAbility.Invoke(ability1, false);
                            return 2;
                        }

                        return 1;
                    }
                }
                else if ((int)doType1 > (int)doType2)
                {
                    if (doType1 == DoType.Steal)
                    {
                        EventManager.Instance.OnTriggerAbility?.Invoke(ability1, false);

                        // Check if the target unit has enough energy for trigger ability.

                        int targetENG = _unit2.Model.Data.Cur.ENG;
                        int consumENG = _unit2.Model.CurrentLevel.ConsumedEnergy.Value;
                        int stolenENG = Steal.StolenEnergy(_unit2, _unit1);

                        if (targetENG + stolenENG >= consumENG)
                        {
                            EventManager.Instance.OnTriggerAbility.Invoke(ability2, false);
                            return 2;
                        }

                        return 1;
                    }
                }

                EventManager.Instance.OnTriggerAbility?.Invoke(ability1, false);
                EventManager.Instance.OnTriggerAbility?.Invoke(ability2, false);
                return 2;
            }

            EventManager.Instance.OnTriggerAbility?.Invoke(ability2, false);
            return 1;
        }
        else if (ability1 != null)
        {
            EventManager.Instance.OnTriggerAbility?.Invoke(ability1, default);
            return 1;
        }

        return 0;
    }



    public bool RegisterFriendAhead(UnitController _friendAhead, TriggerType _triggerType)
    {
        for (int i = 1; i < _friendAhead.TeamSlots.Length; i++)
        {
            var target = _friendAhead.TeamSlots[i].UnitController();
            if (target == null || target == _friendAhead)
                continue;

            if (target.Model.CurrentLevel.TriggerType != _triggerType)
                continue;

            var searchedFriendAhead = _friendAhead.TeamSlots[i - 1].UnitController();
            if (searchedFriendAhead == null)
                continue;

            if (searchedFriendAhead == _friendAhead)
            {
                switch (_triggerType)
                {
                    case TriggerType.BeforeFriendAheadAttacks:
                        return true;

                    case TriggerType.FriendAheadAttacks:
                        PhaseBattleController.Instance.UnitAbilities.Enqueue(target.Ability);
                        return true;

                    case TriggerType.FriendAheadFaints:
                        PhaseBattleController.Instance.UnitAbilities.Enqueue(target.Ability);
                        return true;

                    case TriggerType.FriendAheadTriggerAbility:
                        PhaseBattleController.Instance.UnitAbilities.Enqueue(target.Ability);
                        return true;
                }
            }
        }

        return false;
    }

}

