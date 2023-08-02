
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CardInfo;

public class TalkState : SettedState
{
    public TalkState(CardManager cardManager, CardNode newBaseNode) : base(cardManager, newBaseNode)
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

            case CardType.DIALOGUE:
                Debug.LogWarning("A dialogue should not be here");
                ShowDialogue(clickedNode);
                return;

            default:
                ToDefaultTransitions(clickedNode);
                return;
        }
    }

    public void ShowDialogue(CardNode clickedNode)
    {
        ResetHover(clickedNode, null);
        _stateManager.PushState(new DialogueState(_cardManager, clickedNode));
    }

    public void ToInventoryTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new CloseAnimation(_cardManager), new ToInventoryAnimation(_cardManager, true) };
        LayoutState newState = new InventoryState(_cardManager);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToDefaultTransitions(CardNode clickedNode)
    {
        if (clickedNode == _cardManager.BaseNode) return;

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

    public void ToBackTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new BackAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public void ToRootTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new ToCoverAnimation(_cardManager), new ExitInventoryPileAnimation(_cardManager) };
        LayoutState newState = new CoverState(_cardManager);

        ToTransition(clickedNode, animations, newState);
    }

    public override void StartHover(CardNode hoveredNode)
    {
        hoveredNode.Body.StartOutline();

        if (hoveredNode.Context.CardType is not CardType.DIALOGUE) return;

        hoveredNode.Body.StartHoverTween();
    }

    public override void EndHover(CardNode hoveredNode)
    {
        hoveredNode.Body.EndHoverTween();
        hoveredNode.Body.EndOutline();
    }
}