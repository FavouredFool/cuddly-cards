
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class RetractKeysAnimation : InventoryAnimation
{
    readonly bool _delay;

    public RetractKeysAnimation(CardManager cardManager, bool delay) : base(cardManager)
    {
        _delay = delay;
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;

        entireSequence.Join(_subAnimations.RaiseNodeToHeight(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT)));

        float delayTime = _delay ? _verticalTime : 0f;

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(delayTime)
            .Append(_subAnimations.FanInCardsToRight(inventoryNode[1])));

        return entireSequence;
    }
}
