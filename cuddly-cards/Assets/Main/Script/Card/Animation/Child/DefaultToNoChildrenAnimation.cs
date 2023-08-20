
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class DefaultToNoChildrenAnimation : ChildParentAnimation
{
    public DefaultToNoChildrenAnimation(CardManager cardManager) : base(cardManager) { }

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
        return DOTween.Sequence();
    }

}
