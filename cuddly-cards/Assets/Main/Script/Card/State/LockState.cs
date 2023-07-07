
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class LockState : SettedState
{
    public LockState(CardManager cardManager, CardNode baseNode) : base (cardManager, baseNode)
    {
    }

    public override void HandleIndividualTransitions(CardNode clickedNode)
    {
        CardType cardType = clickedNode.Context.CardType;

        switch (cardType)
        {
            case CardType.INVENTORY:
                return;

            case CardType.KEY:
            case CardType.DIALOGUE:
                PlayNode(clickedNode);
                return;

            case CardType.COVER:
            case CardType.PLACE:
            case CardType.THING:
            case CardType.PERSON:
            case CardType.LOCK:
            default:
                ToDefaultTransitions(clickedNode);
                return;
        }
    }

    void ToDefaultTransitions(CardNode clickedNode)
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

    void ToRootTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new ToCoverAnimation(_cardManager), new RetractKeysAnimation(_cardManager, true), new ExitInventoryPileAnimation(_cardManager) };
        LayoutState newState = new CoverState(_cardManager);

        ToTransition(clickedNode, animations, newState);
    }

    void ToBackTransition(CardNode clickedNode)
    {
        List<CardAnimation> animations = new() { new BackAnimation(_cardManager), new RetractKeysAnimation(_cardManager, true) };
        LayoutState newState = new MainState(_cardManager, clickedNode);

        ToTransition(clickedNode, animations, newState);
    }

    void PlayNode(CardNode clickedNode)
    {
        CardNode baseNode = _cardManager.BaseNode;

        if (baseNode.Context.DesiredKey.Equals(clickedNode.Context.Label))
        {
            NodeCorrect(clickedNode, baseNode);
        }
        else
        {
            NodeWrong();
        }
    }

    void NodeWrong()
    {
        Debug.Log("WRONG!");
    }

    async void NodeCorrect(CardNode clickedNode, CardNode baseNode)
    {
        await LockOpened(baseNode, clickedNode);

        CardNode childNode = baseNode.Children[0];
        // Für die Animation muss ich das im Voraus machen, bevor es erneut von dem MainState gesetzt wird
        _cardManager.BaseNode = childNode;

        List<CardAnimation> animations = new() { new RetractKeysAnimation(_cardManager, false), new OpenAnimation(_cardManager) };
        LayoutState newState = new MainState(_cardManager, childNode);

        ToTransition(childNode, animations, newState);
    }

    public async Task LockOpened(CardNode lockNode, CardNode keyNode)
    {
        RemoveKeyFromTree(keyNode);
        RemoveLockFromTree(lockNode);

        _ = DisintegrateCard(lockNode);
        await DisintegrateCard(keyNode);
        
        Object.Destroy(keyNode.Body.gameObject);
        Object.Destroy(lockNode.Body.gameObject);
    }

    public void RemoveLockFromTree(CardNode lockNode)
    {
        CardNode exposedNode = lockNode.Children[0];
        CardNode parentNode = lockNode.Parent;
        exposedNode.Parent = parentNode;
        parentNode.Children.Remove(lockNode);
        parentNode.Children.Add(exposedNode);
    }

    public void RemoveKeyFromTree(CardNode keyNode)
    {
        keyNode.Parent.Children.Remove(keyNode);
    }

    public async Task DisintegrateCard(CardNode node)
    {
        await node.Body.DisintegrateCard();
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
