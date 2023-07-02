using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager
{
    readonly CardManager _cardManager;

    protected Stack<LayoutState> _states;

    public Stack<LayoutState> States { get { return _states; } }

    public StateManager(CardManager cardManager)
    {
        _cardManager = cardManager;
        _states = new();
    }

    public void StartStates()
    {
        SetState(new CoverState(_cardManager));
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

    internal void HandleHover(CardNode hoveredNode)
    {
        _states.Peek().HandleHover(hoveredNode);
    }

    public void HandleClick(CardNode clickedNode)
    {
        _states.Peek().HandleClick(clickedNode);
    }
}
