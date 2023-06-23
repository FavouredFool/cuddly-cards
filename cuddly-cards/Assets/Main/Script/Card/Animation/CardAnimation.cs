
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


    public CardAnimation(
        CardManager cardManager, CardMover cardMover, CardInventory cardInventory, float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> tweenXFunc, Func<CardNode, int, Tween> tweenYFunc, Func<CardNode, float, Tween> tweenZFunc)
    {
        _cardManager = cardManager;
        _cardMover = cardMover;
        _cardInventory = cardInventory;
        _waitTime = waitTime;
        _horizontalTime = horizontalWaitTime;
        _verticalTime = verticalWaitTime;
        _playSpaceBottomLeft = playSpaceBottomLeft;
        _playSpaceTopRight = playSpaceTopRight;
        _tweenXFunc = tweenXFunc;
        _tweenYFunc = tweenYFunc;
        _tweenZFunc = tweenZFunc;
    }

    public abstract void MoveCardsStatic(CardNode activeNode, CardNode rootNode);
    public abstract Task AnimateCards(CardNode activeNode, CardNode previousActiveNode, CardNode rootNode);
}
