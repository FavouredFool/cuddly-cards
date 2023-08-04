
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FanDialogueAnimation : ChildParentAnimation
{
    public FanDialogueAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween AnimateChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        List<CardNode> childsToBe = activeNode.Children;

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode node = childsToBe[i];

            _cardManager.AddToTopLevelMainPile(childsToBe[i]);

            sequence.Join(
                DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(node))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.FanOutCards(node, i, childsToBe.Count, false))
            );
        }

        return sequence;
    }
}
