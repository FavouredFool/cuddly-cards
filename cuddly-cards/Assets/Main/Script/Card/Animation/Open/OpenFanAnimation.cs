using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenFanAnimation : OpenParentAnimation
{
    public OpenFanAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween MoveChildren(CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        int count = baseNode.Children.Count;
        for (int i = 0; i < count; i++)
        {
            CardNode node = baseNode.Children[i];

            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.FanOutCards(node, i, count, false)));
        }

        return sequence;
    }
}
