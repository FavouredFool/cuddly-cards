using System;
using System.Collections.Generic;
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

    public override void HandleHover(CardNode hoveredNode)
    {
        // outlines pls?
        return;
    }

    
}
