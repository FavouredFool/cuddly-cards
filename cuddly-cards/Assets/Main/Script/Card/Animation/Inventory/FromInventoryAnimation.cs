
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FromInventoryAnimation : InventoryAnimation
{
    readonly float _delay;

    public FromInventoryAnimation(CardManager cardManager, int delay) : base(cardManager)
    {
        _delay = delay switch
        {
            1 => _waitTime + _horizontalTime + _verticalTime,
            2 => _verticalTime,
            _ => 0
        };
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();
        entireSequence.AppendInterval(_delay);

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;


        entireSequence.Append(_subAnimations.RaiseNodeToHeight(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT)));

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.FanInCardsToRight(inventoryNode[i]))
                );
        }

        return entireSequence;
    }
}
