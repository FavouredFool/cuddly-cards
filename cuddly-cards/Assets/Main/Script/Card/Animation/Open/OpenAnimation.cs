using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenAnimation : OpenParentAnimation
{
    public OpenAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween MoveChildren(CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToChild(baseNode.Children[i], baseNode.Children[i]))
                .Append(_subAnimations.MoveNodeYLowerPile(baseNode.Children[i])));
        }

        return sequence;
    }
}
