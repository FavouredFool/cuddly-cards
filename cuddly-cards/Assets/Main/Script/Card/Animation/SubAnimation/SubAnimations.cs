using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class SubAnimations
{
    // All animations atomized
    readonly CardMover _cardMover;
    readonly CardManager _cardManager;
    readonly CardInventory _cardInventory;

    readonly float _waitTime;
    readonly float _horizontalTime;
    readonly float _verticalTime;

    readonly Vector2 _playSpaceBottomLeft;
    readonly Vector2 _playSpaceTopRight;

    readonly Func<CardNode, float, Tween> _tweenXFunc;
    readonly Func<CardNode, int, Tween> _tweenYFunc;
    readonly Func<CardNode, float, Tween> _tweenZFunc;

    public SubAnimations(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = _cardManager.CardMover;
        _cardInventory = _cardManager.CardInventory;
        _waitTime = _cardMover.WaitTime;
        _horizontalTime = _cardMover.HorizontalTime;
        _verticalTime = _cardMover.VerticalTime;
        _playSpaceBottomLeft = _cardMover.GetPlaySpaceBottomLeft();
        _playSpaceTopRight = _cardMover.GetPlaySpaceTopRight();
        _tweenXFunc = _cardMover.TweenX;
        _tweenYFunc = _cardMover.TweenY;
        _tweenZFunc = _cardMover.TweenZ;
    }

    public Tween FanOutChildFromBase(CardNode newChild)
    {
        return DOTween.Sequence()
            .Append(MoveBaseToChild(newChild, newChild))
            .Append(LowerNodePile(newChild));
    }

    public Tween LiftAndMoveChildToBase(CardNode node, CardNode relativeHeightNode)
    {
        return DOTween.Sequence()
            .Append(RaiseNodePileRelative(node, relativeHeightNode))
            .Append(MoveNodeToLeft(node));
    }

    #region Inventory

    public Tween MoveInventoryCardWhileFanning(int height, bool synchronizeLoweringWithFanning)
    {
        float waitTime = _verticalTime + 2 * _horizontalTime + _waitTime;
        Ease ease = _cardMover.VerticalEasing;
        float time = _verticalTime;

        if(synchronizeLoweringWithFanning)
        {
            waitTime -= _horizontalTime;
            ease = _cardMover.HorizontalEasing;
            time = _horizontalTime;
        }

        return DOTween.Sequence()
            .AppendInterval(waitTime)
            .Append(_cardInventory.InventoryNode.Body.transform.DOMoveY(height * CardInfo.CARDHEIGHT, time).SetEase(ease));
    }

    public Tween FanOutCards(CardNode node, int index, int totalChildren, bool fromRight)
    {
        Sequence entireSequence = DOTween.Sequence();

        float totalSpace = _cardMover.PlaySpaceTopRight.x - _cardMover.PlaySpaceBottomLeft.x;
        float fannedCardSpace = totalSpace - 2 * _cardMover.Border;
        float startOffset = fromRight ? _cardMover.PlaySpaceBottomLeft.x + _cardMover.Border : _cardMover.PlaySpaceTopRight.x - _cardMover.Border;
        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildren - 1);

        int directionSign = fromRight ? 1 : -1;
        float endPositionHorizontal = startOffset + directionSign * (totalChildren - index - 1) * CardInfo.CARDWIDTH * cardPercentage;

        entireSequence.Join(DOTween.Sequence()
            .Append(MoveNodeHorizontally(node, startOffset))
            .Append(RotateOffset(node, fromRight))
            .Append(MoveNodeHorizontally(node, endPositionHorizontal))
            .Join(_tweenYFunc(node, 2)));

        return entireSequence;
    }

    public Tween FanInCardsToRight()
    {
        Sequence entireSequence = DOTween.Sequence();
        CardNode inventoryNode = _cardInventory.InventoryNode;

        int totalChildren = inventoryNode.Children.Count;

        for (int i = 0; i < totalChildren; i++)
        {
            CardNode childNode = inventoryNode[i];

            _cardManager.AddToTopLevelMainPile(childNode);

            entireSequence.Join(DOTween.Sequence()
            .Append(MoveNodeToRight(childNode))
            .Join(RaiseNodePileRelative(childNode, inventoryNode))
            .Join(RotateToIdentity(childNode)));
        }

        return entireSequence;
    }

    #endregion

    #region Move Z
    public Tween MoveNodeFarther(CardNode node)
    {
        return _tweenZFunc(node, _playSpaceTopRight.y);
    }

    public Tween MoveNodeNearer(CardNode node)
    {
        return _tweenZFunc(node, _playSpaceBottomLeft.y);
    }

    #endregion

    #region Move X
    public Tween MoveBaseToChild(CardNode movedChild, CardNode positionReferencedChild)
    {
        return _tweenXFunc(movedChild, positionReferencedChild.Parent.Children.IndexOf(positionReferencedChild) * _cardMover.ChildrenDistance - _cardMover.ChildrenStartOffset);
    }

    public Tween MoveNodeHorizontally(CardNode node, float positionX)
    {
        return _tweenXFunc(node, positionX);
    }

    public Tween MoveNodeToLeft(CardNode node)
    {
        return _tweenXFunc(node, _playSpaceBottomLeft.x);
    }

    public Tween MoveNodeToRight(CardNode node)
    {
        return _tweenXFunc(node, _playSpaceTopRight.x);
    }

    public Tween MoveNodeToOutOfFrameRight(CardNode node)
    {
        return _tweenXFunc(node, _playSpaceTopRight.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x));
    }

    public Tween MoveNodeToMiddle(CardNode node)
    {
        return _tweenXFunc(node, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f);
    }

    public Tween MoveChildOneToRight(CardNode node)
    {
        int index = node.Parent.Children.IndexOf(node);
        return _tweenXFunc(node, _cardMover.ChildrenDistance * (index - 1) - _cardMover.ChildrenStartOffset);
    }

    #endregion

    #region Lower and Lift

    public Tween LowerNodePile(CardNode node)
    {
        return RaiseNodeToHeight(node, node.GetNodeCount(CardTraversal.BODY));
    }

    public Tween LiftNodePile(CardNode node)
    {
        return RaiseNodeToHeight(node, node.GetNodeCount(CardTraversal.CONTEXT));
    }

    public Tween RaiseNodePileRelative(CardNode node, CardNode relativeHeightNode)
    {
        return RaiseNodeToHeight(node, node.GetNodeCountUpToNodeInPile(relativeHeightNode, CardTraversal.CONTEXT));
    }

    public Tween RaiseNodeToHeight(CardNode node, int height)
    {
        return _tweenYFunc(node, height);
    }
    #endregion

    #region Miscellaneous

    public Tween RotateOffset(CardNode node, bool fromRight)
    {
        int rotationDirection = fromRight ? -1 : 1;
        return node.Body.transform.DOLocalRotate(new Vector3(0, 0, rotationDirection * _cardMover.InventoryCardRotationAmount), _waitTime).SetEase(_cardMover.HorizontalEasing);
    }

    public Tween RotateToIdentity(CardNode node)
    {
        return node.Body.transform.DOLocalRotate(new Vector3(0, 0, 0), _waitTime).SetEase(_cardMover.HorizontalEasing);
    }

    #endregion

}
