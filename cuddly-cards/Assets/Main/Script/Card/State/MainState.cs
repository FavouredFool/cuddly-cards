
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class MainState : DefaultState
{
    public MainState(CardManager cardManager, CardNode baseNode) :base (cardManager, baseNode)
    {
    }

    public override void HandleIndividualTransitions(CardNode clickedNode)
    {
        CardType cardType = clickedNode.Context.CardType;

        switch (cardType)
        {
            case CardType.INVENTORY:
                ToInventoryTransition(clickedNode);
                return;

            case CardType.KEY:
            case CardType.DIALOGUE:
                CollectNode(clickedNode);
                return;

            case CardType.LOCK:
                ToLockTransition(clickedNode);
                return;

            case CardType.COVER:
            case CardType.PLACE:
            case CardType.THING:
            case CardType.PERSON:
            default:
                ToDefaultTransitions(clickedNode);
                return;
        }
    }

    public void ToDefaultTransitions(CardNode clickedNode)
    {
        if (clickedNode == _cardManager.BaseNode) return;

        if (_cardManager.BaseNode.Children.Contains(clickedNode))
        {
            ToChildTransition(clickedNode);
            return;
        }

        if (_cardManager.BaseNode.Parent == clickedNode)
        {
            ToBackTransition(clickedNode);
            return;
        }

        if (clickedNode == _cardManager.RootNode)
        {
            ToRootTransition(clickedNode);
            return;
        }
    }

    public void CollectNode(CardNode clickedNode)
    {
        _cardInventory.MoveNodeFromMainToInventory(clickedNode);
    }

    public void ToInventoryTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new CloseAnimation(_cardManager), new ToInventoryAnimation(_cardManager) };
        LayoutState newState = new InventoryState(_cardManager);

        ToTransition(clickedNode, animations, newState, true);
    }

    public void ToLockTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new NoChildrenAnimation(_cardManager), new DisplayKeysAnimation(_cardManager) };
        LayoutState newState = new LockState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState, false);
    }

    public void ToChildTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new ChildAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState, false);
    }

    public void ToBackTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new BackAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState, false);
    }

    public void ToRootTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new(){ new ToCoverAnimation(_cardManager), new ExitInventoryPileAnimation(_cardManager) };
        LayoutState newState = new CoverState(_cardManager, _cardManager.RootNode);

        ToTransition(clickedNode, animations, newState, false);
    }

    public async void ToTransition(CardNode clickedNode, List<CardAnimation> animations, LayoutState stateParent, bool shouldPush)
    {
        foreach (CardAnimation animation in animations)
        {
            _animationManager.AddAnimation(animation);
        }

        await _animationManager.PlayAnimations(clickedNode, _cardManager.BaseNode);

        Action<LayoutState> stateFunction = (shouldPush ? _stateManager.PushState : _stateManager.SetState);
        stateFunction(stateParent);
    }
}
