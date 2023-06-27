using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CloseAnimation : CardAnimation
{
    public CloseAnimation(

        CardManager cardManager,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> _tweenXFuncFunc, Func<CardNode, int, Tween> _tweenYFuncFunc, Func<CardNode, float, Tween> _tweenZFuncFunc

        ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, _tweenXFuncFunc, _tweenYFuncFunc, _tweenZFuncFunc) { }


    public async override Task AnimateCards(CardNode activeNode, CardNode previousActiveNode)
    {
        CardNode rootNode = _cardManager.GetRootNode();
        _cardManager.AddToTopLevelMainPile(activeNode);
        _tweenYFunc(activeNode, activeNode.GetNodeCount(CardInfo.CardTraversal.CONTEXT));

        if (activeNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Parent);

            if (activeNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
            }
        }

        foreach(CardNode childNode in activeNode.Children)
        {
            _cardManager.AddToTopLevelMainPile(childNode);

            DOTween.Sequence()
                .Append(_tweenYFunc(childNode, childNode.GetNodeCountUpToNodeInPile(activeNode, CardInfo.CardTraversal.CONTEXT)))
                .Append(_tweenXFunc(childNode, _playSpaceBottomLeft.x));

            
        }

        await DOTween.Sequence()
            .AppendInterval(_verticalTime + _horizontalTime * 2 + _waitTime + 0.01f)
            .OnComplete(() => { })
            .AsyncWaitForCompletion();
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        throw new NotImplementedException();
    }

    public override void MoveCardsStatic(CardNode activeNode)
    {
        CardNode rootNode = _cardManager.GetRootNode();
        // move in deck -> move out inventory

        _cardManager.AddToTopLevelMainPile(activeNode);
        _cardMover.MoveCard(activeNode, _playSpaceBottomLeft);

        if (activeNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Parent);
            _cardMover.MoveCard(activeNode.Parent, new Vector2(_playSpaceBottomLeft.x, _playSpaceTopRight.y));

            if (activeNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                _cardMover.MoveCard(rootNode, _playSpaceTopRight);
            }
        }
    }

    public override void MoveCardsStaticNew(CardNode activeNode)
    {
        throw new NotImplementedException();
    }
}
