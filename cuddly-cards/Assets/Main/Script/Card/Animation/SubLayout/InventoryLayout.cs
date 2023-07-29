using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryLayout : SubLayout
{
    public InventoryLayout(CardManager cardManager) : base(cardManager)
    {
    }

    public override void SetLayoutStatic(CardNode baseNode)
    {
        if (_stateManager.States.Peek() is CoverState)
        {
            _cardMover.MoveCard(_cardInventory.InventoryNode, new Vector2(_cardMover.GetPlaySpaceTopRight().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x), _cardMover.GetPlaySpaceBottomLeft().y));
        }
        else
        {
            _cardMover.MoveCard(_cardInventory.InventoryNode, new Vector2(_cardMover.GetPlaySpaceTopRight().x, _cardMover.GetPlaySpaceBottomLeft().y));
        }

        switch (_stateManager.States.Peek())
        {
            case InventoryState:
                ResetInventoryState();
                break;
            case LockState:
                ResetLockState();
                break;
            default:
                ResetDefaultState();
                break;
        }
    }

    public void ResetInventoryState()
    {
        // fanned out
        CardNode keyParentNode = _cardInventory.KeyParentNode;

        // Set all cardnodes toplevel
        _cardManager.AddToTopLevelMainPile(keyParentNode);
        foreach (CardNode node in keyParentNode.Children)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
        float fannedCardSpace = totalSpace - 2 * _cardMover.Border;

        float offset = _cardMover.GetPlaySpaceBottomLeft().x + _cardMover.Border;
        FanCardsFromInventorySubcardStatic(keyParentNode, offset, fannedCardSpace);
    }

    public void ResetLockState()
    {
        CardNode keyParentNode = _cardInventory.KeyParentNode;

        _cardManager.AddToTopLevelMainPile(keyParentNode);
        foreach (CardNode node in keyParentNode.Children)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
        float fannedCardSpace = (totalSpace - 2 * _cardMover.Border);

        float keyOffset = _cardMover.GetPlaySpaceBottomLeft().x + _cardMover.Border;
        FanCardsFromInventorySubcardStatic(keyParentNode, keyOffset, fannedCardSpace);
    }

    public void ResetDefaultState()
    {
        // kept in
        CardNode keyParentNode = _cardInventory.KeyParentNode;

        // Set no cardnodes toplevel
        keyParentNode.IsTopLevel = false;
        foreach (CardNode node in keyParentNode.Children)
        {
            node.IsTopLevel = false;
        }
    }

    public void FanCardsFromInventorySubcardStatic(CardNode inventorySubcard, float startFanX, float fannedCardSpace)
    {
        int totalChildCards = inventorySubcard.Children.Count;

        _cardMover.MoveCard(inventorySubcard, new Vector2(startFanX + fannedCardSpace, _cardMover.GetPlaySpaceBottomLeft().y));

        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildCards);

        for (int i = 0; i < totalChildCards; i++)
        {
            _cardMover.MoveCard(inventorySubcard[totalChildCards - 1 - i], new Vector2(startFanX + i * CardInfo.CARDWIDTH * cardPercentage, _cardMover.GetPlaySpaceBottomLeft().y));
        }
    }
}
