
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class FromCoverAnimation : CardAnimation
{
    public FromCoverAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;
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
