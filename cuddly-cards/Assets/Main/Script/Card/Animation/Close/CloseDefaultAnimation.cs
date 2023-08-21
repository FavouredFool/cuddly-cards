using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CloseDefaultAnimation : MainAnimation
{
    public CloseDefaultAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (CardNode childNode in baseNode.Children)
        {
            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(childNode, baseNode))
                .Append(_subAnimations.MoveNodeXToLeft(childNode)));
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
