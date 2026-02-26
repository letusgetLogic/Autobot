using System.Collections;
using System.Collections.Generic;

public class ConvertEnergy : AbilityBase
{
    public ConvertEnergy(UnitController _controller, Level _currentLevel, int _seed) 
        : base(_controller, _currentLevel, _seed)
    {
    }

    protected override IEnumerator Activate()
    {
        int energy = Controller.Model.Data.Cur.ENG;
        if (energy < 0)
            yield break;

        Controller.SetEnergy(-energy, false);

        Attribute buff = default;

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
