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
        CardNode inventoryNode = _cardInventory.InventoryNode;

        // Set all cardnodes toplevel
        _cardManager.AddToTopLevelMainPile(inventoryNode[0]);
        foreach (CardNode node in inventoryNode[0].Children)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }
        _cardManager.AddToTopLevelMainPile(inventoryNode[1]);
        foreach (CardNode node in inventoryNode[1].Children)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
        float fannedCardSpace = (totalSpace - 3 * _cardMover.Border) * 0.5f;

        float dialogueOffset = _cardMover.GetPlaySpaceBottomLeft().x + 2 * _cardMover.Border + fannedCardSpace;
        FanCardsFromInventorySubcardStatic(inventoryNode[0], dialogueOffset, fannedCardSpace);

        float keyOffset = _cardMover.GetPlaySpaceBottomLeft().x + _cardMover.Border;
        FanCardsFromInventorySubcardStatic(inventoryNode[1], keyOffset, fannedCardSpace);
    }

    public void ResetLockState()
    {
        // fanned out
        CardNode inventoryNode = _cardInventory.InventoryNode;

        _cardManager.AddToTopLevelMainPile(inventoryNode[1]);
        foreach (CardNode node in inventoryNode[1].Children)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
        float fannedCardSpace = (totalSpace - 2 * _cardMover.Border);

        float keyOffset = _cardMover.GetPlaySpaceBottomLeft().x + _cardMover.Border;
        FanCardsFromInventorySubcardStatic(inventoryNode[1], keyOffset, fannedCardSpace);
    }

    public void ResetDefaultState()
    {
        // kept in
        CardNode inventoryNode = _cardInventory.InventoryNode;

        // Set no cardnodes toplevel
        inventoryNode[0].IsTopLevel = false;
        foreach (CardNode node in inventoryNode[0].Children)
        {
            node.IsTopLevel = false;
        }
        inventoryNode[1].IsTopLevel = false;
        foreach (CardNode node in inventoryNode[1].Children)
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
