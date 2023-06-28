
using DG.Tweening;
using System.Threading.Tasks;
using System;
using UnityEngine;
using static CardInfo;

public abstract class InventoryAnimation : CardAnimation
{
    public InventoryAnimation(

    CardManager cardManager,
    float waitTime, float horizontalWaitTime, float verticalWaitTime,
    Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
    Func<CardNode, float, Tween> __tweenXFuncFuncFunc, Func<CardNode, int, Tween> __tweenYFuncFuncFunc, Func<CardNode, float, Tween> __tweenZFuncFuncFunc

    ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFuncFuncFunc, __tweenYFuncFuncFunc, __tweenZFuncFuncFunc) { }

}
