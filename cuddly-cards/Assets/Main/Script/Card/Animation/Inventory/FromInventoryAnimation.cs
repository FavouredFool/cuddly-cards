
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FromInventoryAnimation : CardAnimation
{
    bool _doDelay;
    public FromInventoryAnimation(CardManager cardManager, bool doDelay) : base(cardManager) {
        _doDelay = doDelay;
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;

        entireSequence.Append(_subAnimations.MoveNodeY(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT)));

        entireSequence.Join(_subAnimations.FanInCardsToRight(_doDelay));

        return entireSequence;
    }
}
