using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class InventoryLayout : SubLayout
{
    public InventoryLayout(CardManager cardManager) : base(cardManager)
    {
    }

    public override void SetLayoutStatic(CardNode baseNode)
    {
        CardNode inventoryNode = _cardInventory.InventoryNode;

        _cardManager.AddToTopLevel(inventoryNode);

        if (_stateManager.States.Peek() is CoverState)
        {
            _cardMover.MoveCard(_cardInventory.InventoryNode, new Vector2(_cardMover.GetPlaySpaceTopRight().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x), _cardMover.GetPlaySpaceBottomLeft().y));
        }
        else
        {
            _cardMover.MoveCard(_cardInventory.InventoryNode, new Vector2(_cardMover.GetPlaySpaceTopRight().x, _cardMover.GetPlaySpaceBottomLeft().y));
        }

        if (_stateManager.States.Peek() is InventoryState or LockState)
        {
            ResetFannedOutState(inventoryNode);
        }

        inventoryNode.Body.SetHeight(inventoryNode.GetNodeCount(CardTraversal.BODY));
    }

    public void ResetFannedOutState(CardNode inventoryNode)
    {
        int count = inventoryNode.Children.Count;

        for (int i = 0; i < count; i++)
        {
            _cardManager.AddToTopLevel(inventoryNode.Children[i]);
            
            _subStatics.FanOutCard(inventoryNode.Children[i], i, count, true);
        }
    }
}
