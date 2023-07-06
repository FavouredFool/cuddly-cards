using System.Collections.Generic;
using UnityEditor.Overlays;

public class StateManager
{
    readonly CardManager _cardManager;

    public Stack<LayoutState> States { get; } = new();

    public StateManager(CardManager cardManager)
    {
        _cardManager = cardManager;
    }

    public void StartStates()
    {
        SetState(new CoverState(_cardManager, _cardManager.RootNode));
    }

    public void SetState(LayoutState state)
    {
        States.Clear();
        States.Push(state);
        state.StartState();
    }

    public void PushState(LayoutState state)
    {
        States.Push(state);
        state.StartState();
    }

    public void PopState()
    {
        States.Pop();
        States.Peek().StartState();
    }

    public void HandleHover(CardNode hoveredNode)
    {
        States.Peek().HandleHover(hoveredNode);
    }

    public void HandleClick(CardNode clickedNode, CardInfo.Click click)
    {
        States.Peek().HandleClick(clickedNode, click);
    }
}
