
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ChildDefaultToDefaultAnimation : ChildParentAnimation
{
    public ChildDefaultToDefaultAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween AnimateOldChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        List<CardNode> childNodes = baseNode.Children;

        int height = 0;

        for (int i = childNodes.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = childNodes[i];

            if (previousChild == activeNode)
            {
                continue;
            }

            height += previousChild.GetNodeCount(CardTraversal.CONTEXT);

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(previousChild, baseNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeZFarther(previousChild))
                .Append(_subAnimations.MoveNodeY(previousChild, height)));
        }

        return sequence;
    }

    public override Tween AnimateChildren(CardNode activeNode, CardNode baseNode)
    {
        List<CardNode> childsToBe = activeNode.Children;

        Sequence sequence = DOTween.Sequence();

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            _cardManager.AddToTopLevel(childsToBe[i]);

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(childsToBe[i], baseNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveOutChildFromBase(childsToBe[i])));
        }

        return sequence;
    }
}
