
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class MainState : SettedState
{
    public MainState(CardManager cardManager, CardNode baseNode) :base (cardManager, baseNode)
    {
    }

    public override void HandleIndividualTransitions(CardNode clickedNode)
    {
        CardType cardType = clickedNode.Context.CardType;

        switch (cardType)
        {
            case CardType.DIALOGUE:
                Debug.LogWarning("Shouldn't be able to click on dialogues");
                return;

            case CardType.INVENTORY:
                ToInventoryTransition(clickedNode);
                return;

            case CardType.DWRAPPER:
                CollectDialogue(clickedNode);
                return;

            case CardType.KEY:
                CollectKey(clickedNode);
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

    public void CollectKey(CardNode clickedNode)
    {
        _cardInventory.MoveKeyFromMainToInventory(clickedNode);
    }

    public void CollectDialogue(CardNode clickedNode)
    {
        _cardDialogue.SpreadDialogues(clickedNode);
    }

    public void ToInventoryTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new CloseAnimation(_cardManager), new ToInventoryAnimation(_cardManager, true) };
        LayoutState newState = new InventoryState(_cardManager);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToLockTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new NoChildrenAnimation(_cardManager), new ToInventoryAnimation(_cardManager, false) };
        LayoutState newState = new LockState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToChildTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new ChildAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToBackTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new BackAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToRootTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new(){ new ToCoverAnimation(_cardManager), new ExitInventoryPileAnimation(_cardManager) };
        LayoutState newState = new CoverState(_cardManager);

        ToTransition(clickedNode, animations, newState);
    }

    public override void StartHover(CardNode hoveredNode)
    {
        hoveredNode.Body.StartOutline();
    }

    public override void EndHover(CardNode hoveredNode)
    {
        hoveredNode.Body.EndOutline();
    }

}
