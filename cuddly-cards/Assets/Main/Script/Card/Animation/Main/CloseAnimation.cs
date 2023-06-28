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

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();
        CardNode rootNode = _cardManager.GetRootNode();
        _cardManager.AddToTopLevelMainPile(activeNode);
        entireSequence.Join(_tweenYFunc(activeNode, activeNode.GetNodeCount(CardInfo.CardTraversal.CONTEXT)));

        if (activeNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Parent);

            if (activeNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
            }
        }

        foreach (CardNode childNode in activeNode.Children)
        {
            _cardManager.AddToTopLevelMainPile(childNode);

            entireSequence.Join(DOTween.Sequence()
                .Append(_tweenYFunc(childNode, childNode.GetNodeCountUpToNodeInPile(activeNode, CardInfo.CardTraversal.CONTEXT)))
                .Append(_tweenXFunc(childNode, _playSpaceBottomLeft.x)));
        }

        return entireSequence;
    }
}
