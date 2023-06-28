
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ExitInventoryPileAnimation : CardAnimation
{
    public ExitInventoryPileAnimation(

        CardManager cardManager,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFuncFuncFunc, Func<CardNode, int, Tween> __tweenYFuncFuncFunc, Func<CardNode, float, Tween> __tweenZFuncFuncFunc

        ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFuncFuncFunc, __tweenYFuncFuncFunc, __tweenZFuncFuncFunc) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        float xInventoryPosition = _playSpaceTopRight.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x);
        Sequence entireSequence = DOTween.Sequence()
            .AppendInterval(_verticalTime + 2 * _horizontalTime + 2 * _waitTime)
            .Append(_tweenXFunc(_cardInventory.GetInventoryNode(), xInventoryPosition));

        return entireSequence;

    }
}
