using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    protected Stack<LayoutState> _states;

    public void Awake()
    {
        _states = new();
    }

    public void SetState(LayoutState state)
    {
        _states.Clear();
        _states.Push(state);
        state.StartState();
    }

    public void PushState(LayoutState state)
    {
        _states.Push(state);
        state.StartState();
    }

    public void PopState()
    {
        _states.Pop();
        _states.Peek().StartState();
    }

    public Stack<LayoutState> GetStates()
    {
        return _states;
    }
}
