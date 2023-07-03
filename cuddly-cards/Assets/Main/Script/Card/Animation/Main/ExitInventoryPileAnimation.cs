
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
        Sequence entireSequence = DOTween.Sequence()
            .AppendInterval(_verticalTime + 2 * _horizontalTime + 2 * _waitTime)
            .Append(_subAnimations.MoveNodeToOutOfFrameRight(_cardInventory.GetInventoryNode()));

        return entireSequence;
    }
}
