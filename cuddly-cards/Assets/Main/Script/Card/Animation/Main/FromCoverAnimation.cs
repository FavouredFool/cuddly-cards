
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
 
        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_subAnimations.MoveNodeXToLeft(rootNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(rootNode, 1)));

        for (int i = 0; i < rootNode.Children.Count; i++)
        {
            CardNode childNode = rootNode.Children[i];

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(childNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToChild(childNode, childNode))
                .Append(_subAnimations.MoveNodeYLowerPile(childNode)));
        }

        return entireSequence;
    }
}
