using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CloseAnimation : CardAnimation
{
    public CloseAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        entireSequence.Join(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode));

        foreach (CardNode childNode in baseNode.Children)
        {
            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(childNode, baseNode))
                .Append(_subAnimations.MoveNodeXToLeft(childNode)));
        }

        return entireSequence;
    }
}
