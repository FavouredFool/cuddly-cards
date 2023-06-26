
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class EnterInventoryPileAnimation : CardAnimation
{
    public EnterInventoryPileAnimation(

        CardManager cardManager,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFuncFuncFunc, Func<CardNode, int, Tween> __tweenYFuncFuncFunc, Func<CardNode, float, Tween> __tweenZFuncFuncFunc

        ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFuncFuncFunc, __tweenYFuncFuncFunc, __tweenZFuncFuncFunc) { }

    public override void MoveCardsStatic(CardNode pressedNode, CardNode rootNode)
    {
        float xInventoryPosition = _playSpaceTopRight.x;
        _cardMover.MoveCard(_cardInventory.GetInventoryNode(), new Vector2(xInventoryPosition, _playSpaceBottomLeft.y));
    }

    public override Task AnimateCards(CardNode mainToBe, CardNode previousMain, CardNode rootNode)
    {
        return Task.CompletedTask;
    }
}
