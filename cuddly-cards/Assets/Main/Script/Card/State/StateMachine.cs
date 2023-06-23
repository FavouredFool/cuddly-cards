using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected LayoutState _state;

    public void SetState(LayoutState state)
    {
        _state = state;
        _state.StartState();
    }

    public LayoutState GetState()
    {
        return _state;
    }
}
