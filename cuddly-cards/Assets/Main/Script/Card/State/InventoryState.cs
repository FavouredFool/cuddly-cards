
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CardInfo;

public class InventoryState : DefaultState
{
    public InventoryState(CardManager cardManager) : base(cardManager)
    {
    }

    public override void StartState()
    {
        _animationManager.SetCardsStatic();
    }

    public override void HandleIndividualTransitions(CardNode clickedNode)
    {
        if (clickedNode.Context.CardType is CardType.KEY or CardType.DIALOGUE)
        {
            return;
        }

        if (clickedNode == _cardInventory.InventoryNode || clickedNode == _cardManager.BaseNode)
        {
            FromInventoryToBaseTransition(clickedNode);
            return;
        }

        if (_cardManager.BaseNode.Parent == clickedNode)
        {
            FromInventoryToBackTransition(clickedNode);
            return;
        }

        if (clickedNode == _cardManager.RootNode)
        {
            FromInventoryToCoverTransition(clickedNode);
            return;
        }
    }

    public void FromInventoryToBaseTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new OpenAnimation(_cardManager), new FromInventoryAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, _cardManager.BaseNode);

        ToTransition(clickedNode, animations, newState);
    }

    public void FromInventoryToBackTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new BackAnimation(_cardManager), new FromInventoryAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    public void FromInventoryToCoverTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new ToCoverAnimation(_cardManager), new FromInventoryAnimation(_cardManager), new ExitInventoryPileAnimation(_cardManager) };
        LayoutState newState = new CoverState(_cardManager);

        ToTransition(clickedNode, animations, newState);
    }


    public override void HandleHover(CardNode hoveredNode)
    {
        ResetHovers(hoveredNode);

        if (hoveredNode == null)
        {
            return;
        }

        if (hoveredNode.Context.CardType is not (CardType.KEY or CardType.DIALOGUE)) return;

        if (hoveredNode.Body.IsHovered) return;

        hoveredNode.Body.StartHoverTween();
    }

    public void ResetHovers(CardNode hoveredNode)
    {
        foreach (CardNode childNode in _cardInventory.InventoryNode.Children.SelectMany(partNode => partNode.Children))
        {
            childNode.Body.ResetHover(hoveredNode);
        }
    }
}
