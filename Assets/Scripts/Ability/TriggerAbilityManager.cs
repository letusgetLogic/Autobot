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

