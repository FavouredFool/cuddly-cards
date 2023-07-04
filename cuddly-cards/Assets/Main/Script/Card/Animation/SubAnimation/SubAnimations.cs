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
            .Append(RaiseNodePileRelative(node, relativeHeightNode))
            .Append(MoveNodeToLeft(node));
    }

    #region Inventory

    public Tween MoveInventoryCardWhileFanning(int height, bool synchronizeLoweringWithFanning)
    {
        float waitTime = _verticalTime + 2 * _horizontalTime + _waitTime;
        Ease ease = _cardMover.GetVerticalEase();
        float time = _verticalTime;

        if(synchronizeLoweringWithFanning)
        {
            waitTime -= _horizontalTime;
            ease = _cardMover.GetHorizontalEase();
            time = _horizontalTime;
        }

        return DOTween.Sequence()
            .AppendInterval(waitTime)
            .Append(_cardInventory.GetInventoryNode().Body.transform.DOMoveY(height * CardInfo.CARDHEIGHT, time).SetEase(ease));
    }

    public Tween FanOutCardsFromRight(CardNode subNode, float startOffset, float fannedCardSpace)
    {
        Sequence entireSequence = DOTween.Sequence();

        subNode.IsTopLevel = true;

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_tweenXFunc(subNode, startOffset))
            .Append(RotateOffset(subNode))
            .Append(_tweenXFunc(subNode, startOffset + fannedCardSpace))
            .Join(_tweenYFunc(subNode, 2)));


        int totalChildren = subNode.Children.Count;

        for (int i = 0; i < totalChildren; i++)
        {
            CardNode childNode = subNode[i];

            float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildren);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_tweenXFunc(childNode, startOffset))
                .Append(RotateOffset(childNode))
                .Append(_tweenXFunc(childNode, startOffset + (totalChildren - 1 - i) * CardInfo.CARDWIDTH * cardPercentage))
                .Join(_tweenYFunc(childNode, 2)));
        }

        return entireSequence;
    }

    public Tween FanInCardsToRight(CardNode subNode)
    {
        Sequence entireSequence = DOTween.Sequence();
        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        subNode.IsTopLevel = true;

        entireSequence.Join(DOTween.Sequence()
            .Append(MoveNodeToRight(subNode))
            .Join(RaiseNodePileRelative(subNode, inventoryNode))
            .Join(RotateToIdentity(subNode)));

        int totalChildren = subNode.Children.Count;

        for (int i = 0; i < totalChildren; i++)
        {
            CardNode childNode = subNode[i];

            childNode.IsTopLevel = true;

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
        return _tweenXFunc(movedChild, positionReferencedChild.Parent.Children.IndexOf(positionReferencedChild) * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset());
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
        return _tweenXFunc(node, _cardMover.GetChildrenDistance() * (index - 1) - _cardMover.GetChildrenStartOffset());
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

    public Tween RotateOffset(CardNode node)
    {
        return node.Body.transform.DOLocalRotate(new Vector3(0, 0, -_cardMover.GetInventoryCardRotationAmount()), _waitTime).SetEase(_cardMover.GetHorizontalEase());
    }

    public Tween RotateToIdentity(CardNode node)
    {
        return node.Body.transform.DOLocalRotate(new Vector3(0, 0, 0), _waitTime).SetEase(_cardMover.GetHorizontalEase());
    }

    #endregion

}
