
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class FromCoverAnimation : MainAnimation
{
    public FromCoverAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween RootAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;

        sequence.Join(DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_subAnimations.MoveNodeXToLeft(rootNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(rootNode, 1)));

        return sequence;
    }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;

        for (int i = 0; i < rootNode.Children.Count; i++)
        {
            CardNode childNode = rootNode.Children[i];

            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(childNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToChild(childNode, childNode))
                .Append(_subAnimations.MoveNodeYLowerPile(childNode)));
        }

        return sequence;
    }
}
