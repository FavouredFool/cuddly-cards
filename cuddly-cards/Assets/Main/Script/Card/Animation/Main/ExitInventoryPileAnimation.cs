
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
        float xInventoryPosition = _playSpaceTopRight.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x);
        Sequence entireSequence = DOTween.Sequence()
            .AppendInterval(_verticalTime + 2 * _horizontalTime + 2 * _waitTime)
            .Append(_tweenXFunc(_cardInventory.GetInventoryNode(), xInventoryPosition));

        return entireSequence;

    }
}
