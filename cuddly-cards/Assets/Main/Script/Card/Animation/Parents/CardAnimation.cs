
using DG.Tweening;
using System.Threading.Tasks;
using System;
using UnityEngine;

public abstract class CardAnimation
{
    protected CardManager _cardManager;
    protected CardMover _cardMover;
    protected CardInventory _cardInventory;

    protected float _waitTime;
    protected float _horizontalTime;
    protected float _verticalTime;

    protected Vector2 _playSpaceBottomLeft;
    protected Vector2 _playSpaceTopRight;

    protected Func<CardNode, float, Tween> _tweenXFunc;
    protected Func<CardNode, int, Tween> _tweenYFunc;
    protected Func<CardNode, float, Tween> _tweenZFunc;


    public CardAnimation(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = _cardManager.GetCardMover();
        _cardInventory = _cardManager.GetCardInventory();
        _waitTime = _cardMover.GetWaitTime();
        _horizontalTime = _cardMover.GetHorizontalTime();
        _verticalTime = _cardMover.GetVerticalTime();
        _playSpaceBottomLeft = _cardMover.GetPlaySpaceBottomLeft();
        _playSpaceTopRight = _cardMover.GetPlaySpaceTopRight();
        _tweenXFunc = _cardMover.TweenX;
        _tweenYFunc = _cardMover.TweenY;
        _tweenZFunc = _cardMover.TweenZ;
    }

    public abstract Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode);
}
