using System;
using System.Collections.Generic;
using static CardInfo;

public abstract class DefaultState : LayoutState
{
    readonly CardNode _newBaseNode;

    protected DefaultState(CardManager cardManager, CardNode newBaseNode) : base(cardManager)
    {
        _newBaseNode = newBaseNode;
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
        _cardManager.BaseNode = _newBaseNode;
        SetStatic();
    }

    public void SetStatic()
    {
        _animationManager.SetCardsStatic();
    }

    public async void ToTransition(CardNode clickedNode, List<CardAnimation> animations, LayoutState stateParent)
    {
        foreach (CardAnimation animation in animations)
        {
            _animationManager.AddAnimation(animation);
        }

        await _animationManager.PlayAnimations(clickedNode, _cardManager.BaseNode);

        Action<LayoutState> stateFunction;

        switch (clickedNode.Context.CardType)
        {
            case CardType.INVENTORY:
                stateFunction = _stateManager.PushState;
                break;

            default:
                stateFunction = _stateManager.SetState;
                break;
        }

        stateFunction(stateParent);
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        // outlines pls?
        return;
    }

    
}
