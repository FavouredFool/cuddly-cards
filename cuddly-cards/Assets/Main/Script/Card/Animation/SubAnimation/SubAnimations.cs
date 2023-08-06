using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class SubAnimations
{
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
            .Append(MoveNodeXToChild(newChild, newChild))
            .Append(MoveNodeYLowerPile(newChild));
    }

    public Tween LiftAndMoveChildToBase(CardNode node, CardNode relativeHeightNode)
    {
        return DOTween.Sequence()
            .Append(MoveNodeYLiftPile(node, relativeHeightNode))
            .Append(MoveNodeXToLeft(node));
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
        }

        return DOTween.Sequence()
            .AppendInterval(waitTime)
            .Append(_cardInventory.InventoryNode.Body.transform.DOMoveY(height * CardInfo.CARDHEIGHT, time).SetEase(ease));
    }

    public Tween FanOutCard(CardNode node, int index, int totalChildren, bool fromRight)
    {
        Sequence entireSequence = DOTween.Sequence();

        float totalSpace = _cardMover.PlaySpaceTopRight.x - _cardMover.PlaySpaceBottomLeft.x;
        float fannedCardSpace = totalSpace - 2 * _cardMover.Border;
        float startOffset = fromRight ? _cardMover.PlaySpaceBottomLeft.x + _cardMover.Border : _cardMover.PlaySpaceTopRight.x - _cardMover.Border;
        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildren - 1);

        int directionSign = fromRight ? 1 : -1;
        float endPositionHorizontal = startOffset + directionSign * (totalChildren - index - 1) * CardInfo.CARDWIDTH * cardPercentage;

        entireSequence.Join(DOTween.Sequence()
            .Append(MoveNodeX(node, startOffset))
            .Append(RotateOffset(node, fromRight))
            .Append(MoveNodeX(node, endPositionHorizontal))
            .Join(_tweenYFunc(node, 1)));

        return entireSequence;
    }

    public Tween FanInCardsToRight(bool doDelay)
    {
        Sequence entireSequence = DOTween.Sequence();
        CardNode inventoryNode = _cardInventory.InventoryNode;

        float delay = doDelay ? _horizontalTime + _waitTime : 0;

        int totalChildren = inventoryNode.Children.Count;

        for (int i = 0; i < totalChildren; i++)
        {
            CardNode childNode = inventoryNode[i];

            _cardManager.AddToTopLevel(childNode);

            entireSequence.Join(DOTween.Sequence()
            .Append(MoveNodeYLiftPile(childNode, inventoryNode))
            .Join(RotateToIdentity(childNode))
            .AppendInterval(delay)
            .Append(MoveNodeXToRight(childNode)));
            
        }

        return entireSequence;
    }

    #endregion

    #region Move Z

    public Tween MoveNodeZ(CardNode node, float positionZ)
    {
        return _tweenZFunc(node, positionZ);
    }

    public Tween MoveNodeZFarther(CardNode node)
    {
        return MoveNodeZ(node, _playSpaceTopRight.y);
    }

    public Tween MoveNodeZNearer(CardNode node)
    {
        return MoveNodeZ(node, _playSpaceBottomLeft.y);
    }

    #endregion

    #region Move X

    public Tween MoveNodeX(CardNode node, float positionX)
    {
        return _tweenXFunc(node, positionX);
    }

    public Tween MoveNodeXToChild(CardNode movedChild, CardNode positionReferencedChild)
    {
        return MoveNodeX(movedChild, positionReferencedChild.Parent.Children.IndexOf(positionReferencedChild) * _cardMover.ChildrenDistance - _cardMover.ChildrenStartOffset);
    }

    public Tween MoveNodeXToLeft(CardNode node)
    {
        return MoveNodeX(node, _playSpaceBottomLeft.x);
    }

    public Tween MoveNodeXToRight(CardNode node)
    {
        return MoveNodeX(node, _playSpaceTopRight.x);
    }

    public Tween MoveNodeXToFarRight(CardNode node)
    {
        return MoveNodeX(node, _playSpaceTopRight.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x));
    }

    public Tween MoveNodeXToMiddle(CardNode node)
    {
        return MoveNodeX(node, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f);
    }

    public Tween MoveNodeXChildOneSpaceToLeft(CardNode node)
    {
        int index = node.Parent.Children.IndexOf(node);
        return MoveNodeX(node, _cardMover.ChildrenDistance * (index - 1) - _cardMover.ChildrenStartOffset);
    }

    #endregion

    #region MoveY

    public Tween MoveNodeY(CardNode node, int height)
    {
        return _tweenYFunc(node, height);
    }

    public Tween MoveNodeYLowerPile(CardNode node)
    {
        return MoveNodeY(node, node.GetNodeCount(CardTraversal.BODY));
    }

    public Tween MoveNodeYLiftPile(CardNode node, CardNode relativeHeightNode)
    {
        return MoveNodeY(node, node.GetNodeCountUpToNodeInPile(relativeHeightNode, CardTraversal.CONTEXT));
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
