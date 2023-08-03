
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ChildAnimation : DeeperParentAnimation
{
    public ChildAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween AnimateChildren(CardNode activeNode, CardNode baseNode)
    {
        List<CardNode> childsToBe = activeNode.Children;

        Sequence sequence = DOTween.Sequence();

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            _cardManager.AddToTopLevelMainPile(childsToBe[i]);

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(childsToBe[i], baseNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.FanOutChildFromBase(childsToBe[i])));
        }

        return sequence;
    }
}
