using UnityEngine;

public class AI : MonoBehaviour, IFiniteStateMachine
{
    private StateBase state;

    private void Awake()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public void SetState(StateBase _state)
    {
        throw new System.NotImplementedException();
    }

    public void SetSubState(StateBase _state)
    {
        throw new System.NotImplementedException();
    }
}
