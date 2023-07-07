using System;
using System.Collections.Generic;
using System.Linq;
using static CardInfo;

public abstract class DefaultState : LayoutState
{
    protected DefaultState(CardManager cardManager) : base(cardManager)
    {
    }

    public abstract void HandleIndividualTransitions(CardNode clickedNode);

    public override void HandleClick(CardNode clickedNode, Click click)
    {
        if (clickedNode == null)
        {
            return;
        }

        ResetHover(clickedNode, null);

        if (click == Click.RIGHT)
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode));
            return;
        }

        HandleIndividualTransitions(clickedNode);
    }

    public override void StartState()
    {
        SetStatic();
    }

    public void SetStatic()
    {
        _animationManager.SetCardsStatic();
    }

    public async void ToTransition(CardNode clickedNode, List<CardAnimation> animations, LayoutState state)
    {
        foreach (CardAnimation animation in animations)
        {
            _animationManager.AddAnimation(animation);
        }

        await _animationManager.PlayAnimations(clickedNode, _cardManager.BaseNode);

        _stateManager.SetState(state);
    }

    public abstract void StartHover(CardNode hoveredNode);

    public abstract void EndHover(CardNode hoveredNode);

    public override void HandleHover(CardNode hoveredNode)
    {
        ResetHovers(hoveredNode);

        if (hoveredNode == null) return;

        if (hoveredNode.Body.IsHovered) return;

        hoveredNode.Body.IsHovered = true;

        StartHover(hoveredNode);
    }

    public void ResetHovers(CardNode hoveredNode)
    {
        foreach (CardNode childNode in _cardManager.GetClickableNodes())
        {
            ResetHover(childNode, hoveredNode);
        }
    }

    public void ResetHover(CardNode childNode, CardNode hoveredNode)
    {
        if (childNode == hoveredNode) return;

        if (!childNode.Body.IsHovered) return;

        childNode.Body.IsHovered = false;

        EndHover(childNode);
    }

}
