using System.Runtime.CompilerServices;
using UnityEngine;

public class AI : MonoBehaviour, I_FSM_AI
{
    public static readonly string Name = "Octobot";

    private StateBaseAI state;

    private UnitController[] shopBotDatas;
    private UnitController[] shopItemDatas;

    private void Awake()
    {
        StartShop();
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public void SetState(StateBaseAI _state)
    {
        if (state != null)
            state.OnExit(this);

        state = _state;

        if (_state == null)
            return;

        state.OnEnter(this);
    }

    private void StartShop()
    {

        SetState(new AI_SearchInFactory());
    }
}
