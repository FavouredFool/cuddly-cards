using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CloseFanAnimation : MainAnimation
{
    public CloseFanAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(AnimateOldChildrenChildren(activeNode, baseNode));

        foreach (CardNode childNode in baseNode.Children)
        {
            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.FanInCard(childNode, baseNode, false, false)));
        }

        return sequence;
    }

    public Tween AnimateOldChildrenChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (CardNode child in baseNode.Children)
        {
            foreach (CardNode childChild in child.Children)
            {
                _cardManager.AddToTopLevel(childChild);

                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeY(childChild, childChild.GetNodeCountUpToNodeInPile(baseNode, CardTraversal.CONTEXT)))
                    );
            }
        }

        return sequence;
    }

    public override Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode));

        return sequence;
    }
}
