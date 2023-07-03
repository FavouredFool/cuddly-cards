using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class SubAnimations
{
    // All animations atomized
    CardMover _cardMover;
    CardManager _cardManager;
    CardInventory _cardInventory;

    SubAnimations _subAnimations;

    float _waitTime;
    float _horizontalTime;
    float _verticalTime;

    Vector2 _playSpaceBottomLeft;
    Vector2 _playSpaceTopRight;

    Func<CardNode, float, Tween> _tweenXFunc;
    Func<CardNode, int, Tween> _tweenYFunc;
    Func<CardNode, float, Tween> _tweenZFunc;

    public SubAnimations(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = _cardManager.CardMover;
        _cardInventory = _cardManager.CardInventory;
        _waitTime = _cardMover.GetWaitTime();
        _horizontalTime = _cardMover.GetHorizontalTime();
        _verticalTime = _cardMover.GetVerticalTime();
        _playSpaceBottomLeft = _cardMover.GetPlaySpaceBottomLeft();
        _playSpaceTopRight = _cardMover.GetPlaySpaceTopRight();
        _tweenXFunc = _cardMover.TweenX;
        _tweenYFunc = _cardMover.TweenY;
        _tweenZFunc = _cardMover.TweenZ;
    }

    public Tween FanOutChildFromBase(CardNode newChild, CardNode previousActiveNode)
    {
        return DOTween.Sequence()
            .Append(MoveBaseToChild(newChild, newChild))
            .Append(LowerNodePile(newChild));
    }

    public Tween LiftAndMoveChildToBase(CardNode node, CardNode relativeHeightNode)
    {
        return DOTween.Sequence()
            .Append(MoveNodePileRelative(node, relativeHeightNode))
            .Append(MoveChildToBase(node));
    }

    #region Move Z
    public Tween MoveBaseToBack(CardNode node)
    {
        return _tweenZFunc(node, _playSpaceTopRight.y);
    }
    #endregion

    #region Move X
    public Tween MoveBaseToChild(CardNode movedChild, CardNode positionReferencedChild)
    {
        return _tweenXFunc(movedChild, positionReferencedChild.Parent.Children.IndexOf(positionReferencedChild) * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset());
    }

    public Tween MoveChildToBase(CardNode node)
    {
        return _tweenXFunc(node, _playSpaceBottomLeft.x);
    }

    public Tween MoveChildToDiscard(CardNode node)
    {
        return _tweenXFunc(node, _playSpaceTopRight.x);
    }

    #endregion

    #region Lower and Lift

    public Tween LowerNodePile(CardNode node)
    {
        return MoveNodeToHeight(node, node.GetNodeCount(CardTraversal.BODY));
    }

    public Tween MoveNodeToHeight(CardNode node, int height)
    {
        return _tweenYFunc(node, height);
    }

    public Tween LiftNodePile(CardNode node)
    {
        return _tweenYFunc(node, node.GetNodeCount(CardTraversal.CONTEXT));
    }

    public Tween MoveNodePileRelative(CardNode node, CardNode relativeHeightNode)
    {
        return _tweenYFunc(node, node.GetNodeCountUpToNodeInPile(relativeHeightNode, CardTraversal.CONTEXT));
    }
    #endregion
}
