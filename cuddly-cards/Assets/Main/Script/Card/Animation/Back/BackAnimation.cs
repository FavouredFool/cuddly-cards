
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackAnimation : BackParentAnimation
{
    public BackAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (CardNode child in activeNode.Children)
        {
            _cardManager.AddToTopLevel(child);

            if (child == baseNode)
            {
                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(baseNode, activeNode))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(_subAnimations.MoveNodeXToChild(baseNode, baseNode))
                    .Append(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode)));

                foreach (CardNode childchild in child.Children)
                {
                    sequence.Join(DOTween.Sequence()
                        .Append(_subAnimations.MoveNodeYLiftPile(childchild, activeNode))
                        .Append(_subAnimations.MoveNodeXToLeft(childchild))
                        .AppendInterval(_waitTime)
                        .Append(_subAnimations.MoveNodeXToChild(childchild, baseNode))
                        .Append(_subAnimations.MoveNodeYLiftPile(childchild, baseNode)));
                }

                continue;
            }

            sequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(child, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(child))
            .AppendInterval(_waitTime)
            .Append(_subAnimations.MoveNodeXToChild(child, child))
            .Append(_subAnimations.MoveNodeYLowerPile(child)));
        }

        return sequence;
    }
}
