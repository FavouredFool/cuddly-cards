
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class LockState : DefaultState
{
    public LockState(CardManager cardManager, CardNode baseNode) : base (cardManager, baseNode)
    {
    }

    public override async void HandleIndividualTransitions(CardNode clickedNode)
    {
        CardType cardType = clickedNode.Context.CardType;

        switch (cardType)
        {
            case CardType.INVENTORY:
                return;

            case CardType.KEY:
            case CardType.DIALOGUE:

                // Compare the clicked nodes name to the desired KEy

                CardNode baseNode = _cardManager.BaseNode;

                if (baseNode.Context.DesiredKey.Equals(clickedNode.Context.Label))
                {
                    await LockOpened(baseNode, clickedNode);

                    CardNode childNode = baseNode.Children[0];

                    _animationManager.AddAnimation(new RetractKeysAnimation(_cardManager, false));
                    _animationManager.AddAnimation(new OpenAnimation(_cardManager));

                    await _animationManager.PlayAnimations(childNode);

                    // Animation
                    _stateManager.SetState(new MainState(_cardManager, childNode));

                    return;
                }
                else
                {
                    Debug.Log("WRONG!!");
                }
                return;

            case CardType.COVER:
            case CardType.PLACE:
            case CardType.THING:
            case CardType.PERSON:
            case CardType.LOCK:
            default:
                EvaluateDefaultCardAction(clickedNode);
                return;
        }
    }

    public async void EvaluateDefaultCardAction(CardNode clickedNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        CardNode previousActiveNode = _cardManager.BaseNode;

        LayoutState nextState;

        if (clickedNode == previousActiveNode)
        {
            return;
        }
        else if (previousActiveNode.Parent == clickedNode)
        {
            _animationManager.AddAnimation(new BackAnimation(_cardManager));
            _animationManager.AddAnimation(new RetractKeysAnimation(_cardManager, true));

            nextState = new MainState(_cardManager, clickedNode);

        }
        else if (clickedNode == rootNode)
        {
            _animationManager.AddAnimation(new ToCoverAnimation(_cardManager));
            _animationManager.AddAnimation(new RetractKeysAnimation(_cardManager, true));
            _animationManager.AddAnimation(new ExitInventoryPileAnimation(_cardManager));

            nextState = new CoverState(_cardManager, rootNode);
        }
        else
        {
            Debug.LogError("Pressed something weird");
            return;
        }

        await _animationManager.PlayAnimations(clickedNode, previousActiveNode);

        _stateManager.SetState(nextState);

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

    public async Task LockOpened(CardNode lockNode, CardNode keyNode)
    {
        // destroy lock and Node visually
        // -> Layout should change instantly

        // 1. Lock and Key disintegrate -> elements get rewired in node-tree
        RemoveKeyFromTree(keyNode);
        RemoveLockFromTree(lockNode);


        _ = DisintegrateCard(lockNode);
        await DisintegrateCard(keyNode);
        

        Object.Destroy(keyNode.Body.gameObject);
        Object.Destroy(lockNode.Body.gameObject);
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
