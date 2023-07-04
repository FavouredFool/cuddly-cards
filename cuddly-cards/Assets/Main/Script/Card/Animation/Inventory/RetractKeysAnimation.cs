
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class RetractKeysAnimation : InventoryAnimation
{
    public RetractKeysAnimation(CardManager cardManager) : base(cardManager)
    {
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        entireSequence.Join(_subAnimations.RaiseNodeToHeight(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT)));

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_subAnimations.FanInCardsToRight(inventoryNode[1])));

        return entireSequence;
    }
}
