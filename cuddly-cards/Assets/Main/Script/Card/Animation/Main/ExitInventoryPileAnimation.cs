
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static CardInfo;

public class ExitInventoryPileAnimation : CardAnimation
{
    public ExitInventoryPileAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        entireSequence.Join(GetMoveOutOfFrameTween(_cardManager.CardInventory.InventoryNode));

        foreach (CardNode subPartNode in _cardManager.CardInventory.InventoryNode.Children)
        {
            if (subPartNode.IsTopLevel)
            {
                entireSequence.Join(GetMoveOutOfFrameTween(subPartNode));
            }

            foreach (var childNode in subPartNode.Children.Where(childNode => childNode.IsTopLevel))
            {
                entireSequence.Join(GetMoveOutOfFrameTween(childNode));
            }
        }

        return entireSequence;
    }

    Tween GetMoveOutOfFrameTween(CardNode node)
    {
        return DOTween.Sequence()
                    .AppendInterval(_verticalTime + 2 * _horizontalTime + 2 * _waitTime)
                    .Append(_subAnimations.MoveNodeToOutOfFrameRight(node));
    }
}
