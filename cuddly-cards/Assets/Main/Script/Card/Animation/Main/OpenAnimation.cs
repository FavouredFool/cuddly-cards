using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenAnimation : CardAnimation
{
    public OpenAnimation(

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

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_horizontalTime)
            .Append(_tweenYFunc(activeNode, 1)));

        if (activeNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Parent);

            if (activeNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
            }
        }

        for (int i = 0; i < activeNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(activeNode.Children[i]);

            entireSequence.Join(DOTween.Sequence()
                .Append(_tweenXFunc(activeNode.Children[i], i * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                .Append(_tweenYFunc(activeNode.Children[i], activeNode.Children[i].GetNodeCount(CardInfo.CardTraversal.CONTEXT))));
        }
        return entireSequence;
    }
}
