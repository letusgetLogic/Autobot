using System.Collections;
using UnityEngine;

public class InitializeState : StateBaseBattle
{
    private Player player1;
    private Player player2;

    /// <summary>
    /// Constructor of InitializeState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public InitializeState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(I_FSM_Battle _ctx)
    {
        Debug.Log("--- InitState - Battle Phase " + GameManager.Instance.CurrentGame.SavedRounds.Count);


        if (GameManager.Instance.Players.Count < 2)
        {
            Debug.LogWarning("Players.Count = " + GameManager.Instance.Players.Count);
            _ctx.SetState(null);
            return;
        }

        player1 = GameManager.Instance.Players[0];
        player2 = GameManager.Instance.Players[1];

        PhaseBattleController.Instance.StartCoroutine(Initialize());
    }

    public override void OnUpdate(I_FSM_Battle _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
        else
        {
            _ctx.SetState(new CheckOutcomeState(
                PhaseBattleController.Instance.Process.DurationCheckOutcome));
        }
    }

    /// <summary>
    /// Initializes the unit team of players.
    /// </summary>
    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => PhaseBattleView.Instance != null);

        if (GameManager.Instance.Replay != null)
        {
            var data1 = new PlayerData(GameManager.Instance.CurrentRound.SavedPlayerData1);
            var data2 = new PlayerData(GameManager.Instance.CurrentRound.SavedPlayerData2);

            PhaseBattleView.Instance.Initialize(data1, data2);
            SpawnUnitsByReplay(data1, PhaseBattleController.Instance.Slots1(), true);
            SpawnUnitsByReplay(data2, PhaseBattleController.Instance.Slots2(), false);

            yield break;
        }

        PhaseBattleView.Instance.Initialize(player1.Data, player2.Data);
        SpawnUnits(player1, PhaseBattleController.Instance.Slots1(), true);
        SpawnUnits(player2, PhaseBattleController.Instance.Slots2(), false);
    }

    /// <summary>
    /// Instantiates and initializes the units.
    /// </summary>
    private void SpawnUnits(Player _player, Slot[] _slots, bool _isLeft)
    {
        if (_player.Data.TeamUnitDatas == null)
        {
            Debug.LogWarning($"{_player.Data.Name} TeamUnitDatas is null");
            return;
        }

        if (_player.Data.TeamUnitDatas.Length < _slots.Length)
        {
            Debug.LogWarning($"{_player.Data.Name} TeamUnitDatas.Length < Slots.Length");
            return;
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            var unitData = _player.Data.TeamUnitDatas[i];
            if (unitData != null && unitData.Cur.HP > 0)
            {
                var unitController = SpawnManager.Instance.Spawn(
                    PackManager.Instance.GetSoUnit(unitData).soUnit,
                    PackManager.Instance.GetSoUnit(unitData).index,
                    unitData,
                    UnitState.InPhaseBattle,
                    _slots[i].transform,
                    _isLeft);

                Debug.Log(unitController.name + " init is left " + unitController.Model.Data.IsTeamLeft);
            }
        }
    }

    /// <summary>
    ///  Instantiates and initializes the units by replay.
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="_slots"></param>
    /// <param name="_isLeft"></param>
    private void SpawnUnitsByReplay(PlayerData _data, Slot[] _slots, bool _isLeft)
    {
        if (_data.TeamUnitDatas == null)
        {
            Debug.LogWarning($"{_data.Name} TeamUnitDatas is null");
            return;
        }

        if (_data.TeamUnitDatas.Length < _slots.Length)
        {
            Debug.LogWarning($"{_data.Name} TeamUnitDatas.Length < Slots.Length");
            return;
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            var unitData = _data.TeamUnitDatas[i];
            if (unitData != null && unitData.Cur.HP > 0)
            {
                var unitController = SpawnManager.Instance.Spawn(
                    PackManager.Instance.GetSoUnit(unitData).soUnit,
                    PackManager.Instance.GetSoUnit(unitData).index,
                    unitData,
                    UnitState.InPhaseBattle,
                    _slots[i].transform,
                    _isLeft);
            }
        }
    }
}