
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ExitInventoryPileAnimation : CardAnimation
{
    public ExitInventoryPileAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        entireSequence.Join(GetMoveOutOfFrameTween(_cardInventory.GetInventoryNode()));

        foreach (CardNode subPartNode in _cardInventory.GetInventoryNode().Children)
        {
            if (subPartNode.IsTopLevel)
            {
                entireSequence.Join(GetMoveOutOfFrameTween(subPartNode));
            }

            foreach (CardNode childNode in subPartNode.Children)
            {
                if (childNode.IsTopLevel)
                {
                    entireSequence.Join(GetMoveOutOfFrameTween(childNode));
                }
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
