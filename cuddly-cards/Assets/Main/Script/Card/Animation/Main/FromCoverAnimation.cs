
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class FromCoverAnimation : CardAnimation
{
    public FromCoverAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;
        _cardManager.AddToTopLevelMainPile(rootNode);
        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_subAnimations.MoveNodeToLeft(rootNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.RaiseNodeToHeight(rootNode, 1)));

        for (int i = 0; i < rootNode.Children.Count; i++)
        {
            CardNode childNode = rootNode.Children[i];
            _cardManager.AddToTopLevelMainPile(childNode);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeToLeft(childNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveBaseToChild(childNode, childNode))
                .Append(_subAnimations.LowerNodePile(childNode)));
        }

        return entireSequence;
    }
}
