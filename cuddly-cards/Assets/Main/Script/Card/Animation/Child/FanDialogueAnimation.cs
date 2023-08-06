using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FanDialogueAnimation : ChildParentAnimation
{
    int _height = 0;

    public FanDialogueAnimation(CardManager cardManager) : base(cardManager) {}

    public override Tween AnimateChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> childChildList = new();

        for (int i = 0; i < childsToBe.Count; i++)
        {
            CardNode node = childsToBe[i];

            foreach (CardNode childChild in node.Children)
            {
                _cardManager.AddToTopLevel(childChild, false);
                childChildList.Add(childChild);
            }

            _cardManager.AddToTopLevel(node);

            sequence.Join(
                DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(node))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.FanOutCards(node, i, childsToBe.Count, false))
            );
        }

        for (int i = childChildList.Count-1; i >= 0; i--)
        {
            CardNode node = childChildList[i];

            _height += node.GetNodeCount(CardTraversal.CONTEXT);

            sequence.Join(
                DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(node))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeY(node, _height)));
        }

        return sequence;
    }

    public override Tween MoveBaseNode(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Append(_subAnimations.LiftAndMoveChildToBase(activeNode, baseNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(activeNode, _height + 1));
    }
}
