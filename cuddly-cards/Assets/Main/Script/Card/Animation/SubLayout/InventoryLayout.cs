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
        }
    }

    public void ResetInventoryState()
    {
    foreach (CardNode node in _cardInventory.InventoryNode.Children)
    {
        _cardManager.AddToTopLevelMainPile(node);
    }

    float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
    float fannedCardSpace = totalSpace - 2 * _cardMover.Border;

    float offset = _cardMover.GetPlaySpaceBottomLeft().x + _cardMover.Border;
    FanCardsFromInventorySubcardStatic(offset, fannedCardSpace);
    }

    public void ResetLockState()
    {
        foreach (CardNode node in _cardInventory.InventoryNode.Children)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
        float fannedCardSpace = (totalSpace - 2 * _cardMover.Border);

        float keyOffset = _cardMover.GetPlaySpaceBottomLeft().x + _cardMover.Border;
        FanCardsFromInventorySubcardStatic(keyOffset, fannedCardSpace);
    }

    public void FanCardsFromInventorySubcardStatic(float startFanX, float fannedCardSpace)
    {
        CardNode inventoryNode = _cardInventory.InventoryNode;

        int totalChildCards = inventoryNode.Children.Count;

        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildCards - 1);

        for (int i = 0; i < totalChildCards; i++)
        {
            _cardMover.MoveCard(inventoryNode[totalChildCards - i - 1], new Vector2(startFanX + i * CardInfo.CARDWIDTH * cardPercentage, _cardMover.GetPlaySpaceBottomLeft().y));
        }
    }
}
