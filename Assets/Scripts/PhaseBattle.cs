using System.Collections;
using TMPro;
using UnityEngine;

public class PhaseBattle : MonoBehaviour
{
    public static PhaseBattle Instance { get; private set; }

    [Header("Setting")]
    [SerializeField]
    private float delayInsert = 2.0f;
    [SerializeField]
    private float durationInsert = 1.0f;
    [SerializeField]
    private float delayDeath = 1.0f;

    [Header("Player left")]
    [SerializeField]
    private TextMeshProUGUI name1;
    [SerializeField]
    private TextMeshProUGUI turn1, wins1, lives1;
    [SerializeField]
    private Slot[] slots1;

    [Header("Player right")]
    [SerializeField]
    private TextMeshProUGUI name2;
    [SerializeField]
    private TextMeshProUGUI turn2, wins2, lives2;
    [SerializeField]
    private Slot[] slots2;

    private Template player1, player2;
    private UnitController unit1, unit2;

    public enum State { None, Init, Insert, Start, CheckOutcome, Attack, CheckDeath }
    public State MyState { get; set; }
    private bool isAttackOnce = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private void Update()
    {
        switch (MyState)
        {
            case State.Init:
                StartCoroutine(SetState(State.Insert, delayInsert));
                MyState = State.None;
                break;
            case State.Insert:
                MoveCloserTogether(slots1);
                MoveCloserTogether(slots2);
                StartCoroutine(SetState(State.CheckOutcome, durationInsert));
                MyState = State.None;
                break;
            case State.Start:
                MyState = State.CheckOutcome;
                break;
            case State.CheckOutcome:
                CheckOutcome();
                break;
            case State.Attack:
                //if (isAttackOnce == true)
                //{
                //    isAttackOnce = false;
                    AttackEachOther();
                //}
                StartCoroutine(SetState(State.CheckDeath, delayDeath));
                MyState = State.None;
                break;
            case State.CheckDeath:
                CheckDeath();
                StartCoroutine(SetState(State.Insert, delayInsert));
                MyState = State.None;
                break;
        }
    }

    /// <summary>
    /// Initializes the players.
    /// </summary>
    /// <param name="_player1"></param>
    /// <param name="_player2"></param>
    public void Initialize(Template _player1, Template _player2)
    {
        MyState = State.Init;

        player1 = _player1;
        name1.text = player1.Name;
        turn1.text = player1.Turns.ToString();
        wins1.text = player1.Wins.ToString();
        lives1.text = player1.Lives.ToString();
        player2 = _player2;
        name2.text = player2.Name;
        turn2.text = player2.Turns.ToString();
        wins2.text = player2.Wins.ToString();
        lives2.text = player2.Lives.ToString();

        SetUnitsToPosition();
    }

    private IEnumerator SetState(State _state, float delay)
    {
        yield return new WaitForSeconds(delay);

        MyState = _state;
    }

    /// <summary>
    /// Instantiates the units.
    /// </summary>
    private void SetUnitsToPosition()
    {
        SetUnitsToPosition(player1, slots1, false);
        SetUnitsToPosition(player2, slots2, true);
    }

    /// <summary>
    /// Instantiates the units.
    /// </summary>
    private void SetUnitsToPosition(Template player, Slot[] slots, bool isRight)
    {
        for (int i = 0; i < player.BattleSlots.Length; i++)
        {
            var unit = player.BattleSlots[i].Unit();
            if (unit != null)
            {
                var unitOnScene = Instantiate(unit);
                unitOnScene.GetComponent<UnitController>().SetModel(unit.GetComponent<UnitController>().Model);
                unitOnScene.transform.SetParent(slots[i].transform, false);

                if (isRight)
                    unitOnScene.GetComponent<UnitView>().SetRightSide();
            }
        }
    }

    private void MoveCloserTogether(Slot[] slots)
    {
        for (int i = 1; i < slots.Length; i++)
        {
            var movedUnit = slots[i].Unit();

            if (movedUnit == null || slots[i - 1].Unit() != null)
                continue;

            bool isDone = false;
            int target = i;
            do
            {
                movedUnit.transform.SetParent(slots[i - 1].transform, false);

                if (i - 1 == 0)
                    isDone = true;
                else
                {
                    if (slots[i - 2].Unit() == null)
                        i--;
                    else
                    {
                        isDone = true;
                    }
                }
            }
            while (!isDone);

            i = target;
        }
    }

    private void CheckOutcome()
    {
        // Draw
        if (slots1[0].Unit() == null)
        {
            if (slots2[0].Unit() == null)
            {
                GameManager.Instance.UpdatePlayerStats(0);
            }
            else
            {
                GameManager.Instance.UpdatePlayerStats(1);
            }
            MyState = State.None;
        }
        else
        {
            if (slots2[0].Unit() == null)
            {
                GameManager.Instance.UpdatePlayerStats(-1);
                MyState = State.None;
            }
            else
            {
                isAttackOnce = true;
                MyState = State.Attack;
            }
        }
    }

    private void AttackEachOther()
    {
        unit1 = slots1[0].UnitController();
        unit2 = slots2[0].UnitController();

        unit1.TakeDamage(unit2.Model.BattleAttack);
        unit2.TakeDamage(unit1.Model.BattleAttack);
    }

    /// <summary>
    /// Checks death, if true destroy game object of unit.
    /// </summary>
    private void CheckDeath()
    {
        if(unit1.Model.BattleHealth <= 0)
            Destroy(unit1.gameObject);

        if(unit2.Model.BattleHealth <= 0)
            Destroy(unit2.gameObject);
    }
}
