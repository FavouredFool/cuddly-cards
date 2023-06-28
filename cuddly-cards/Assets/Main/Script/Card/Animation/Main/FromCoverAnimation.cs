
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class FromCoverAnimation : CardAnimation
{
    public FromCoverAnimation(

        CardManager cardManager,
        float waitTime, float horizontalWaitTime, float verticalWaitTime,
        Vector2 playSpaceBottomLeft, Vector2 playSpaceTopRight,
        Func<CardNode, float, Tween> __tweenXFunc, Func<CardNode, int, Tween> __tweenYFunc, Func<CardNode, float, Tween> __tweenZFunc

        ) : base(cardManager, waitTime, horizontalWaitTime, verticalWaitTime, playSpaceBottomLeft, playSpaceTopRight, __tweenXFunc, __tweenYFunc, __tweenZFunc) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.GetRootNode();
        _cardManager.AddToTopLevelMainPile(rootNode);
        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_tweenXFunc(rootNode, _playSpaceBottomLeft.x))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_tweenYFunc(rootNode, 1)));

        for (int i = 0; i < rootNode.Children.Count; i++)
        {
            CardNode childNode = rootNode.Children[i];
            _cardManager.AddToTopLevelMainPile(childNode);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_tweenXFunc(childNode, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(_tweenXFunc(childNode, i * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset()))
                .Append(_tweenYFunc(childNode, childNode.GetNodeCount(CardTraversal.CONTEXT))));
        }

        return entireSequence;
    }
}
