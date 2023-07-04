
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FromInventoryAnimation : InventoryAnimation
{
    public FromInventoryAnimation(CardManager cardManager) : base(cardManager)
    {
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;


        entireSequence.Join(_subAnimations.RaiseNodeToHeight(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT)));

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.FanInCardsToRight(inventoryNode[i]))
                );
        }

        return entireSequence;
    }
}
