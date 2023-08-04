
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackFanAnimation : BackParentAnimation
{
    public BackFanAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        int count = activeNode.Children.Count;
        for (int i = 0; i < count; i++)
        {
            CardNode child = activeNode.Children[i];

            _cardManager.AddToTopLevel(child);

            if (child == baseNode)
            {
                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(baseNode, activeNode))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(_subAnimations.FanOutCards(child, i, count, false)));

                foreach (CardNode childchild in child.Children)
                {
                    sequence.Join(DOTween.Sequence()
                        .Append(_subAnimations.MoveNodeYLiftPile(childchild, activeNode))
                        .Append(_subAnimations.MoveNodeXToLeft(childchild))
                        .AppendInterval(_waitTime)
                        .Append(_subAnimations.FanOutCards(childchild, i, count, false)));
                }

                continue;
            }

            sequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(child, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(child))
            .AppendInterval(_waitTime)
            .Append(_subAnimations.FanOutCards(child, i, count, false)));
        }

        return sequence;
    }

}
