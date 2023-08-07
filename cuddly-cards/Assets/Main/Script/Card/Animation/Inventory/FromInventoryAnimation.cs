
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FromInventoryAnimation : InventoryAnimation
{
    bool _doDelay;
    public FromInventoryAnimation(CardManager cardManager, bool doDelay) : base(cardManager) {
        _doDelay = doDelay;
    }

    public override Tween InventoryCardAnimation(CardNode inventoryNode)
    {
        return _subAnimations.MoveNodeY(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT));
    }

    public override Tween KeysAnimation(CardNode inventoryNode)
    {
        return _subAnimations.FanInCardsToRight(_doDelay);
    }
}
