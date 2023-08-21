using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ChildDefaultToFanAnimation : ChildParentAnimation
{
    int _height = 0;

    public ChildDefaultToFanAnimation(CardManager cardManager) : base(cardManager) {}

    public override Tween AnimateOldChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        List<CardNode> childNodes = baseNode.Children;

        int height = 0;

        for (int i = childNodes.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = childNodes[i];

            if (previousChild == activeNode)
            {
                continue;
            }

            height += previousChild.GetNodeCount(CardTraversal.CONTEXT);

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(previousChild, baseNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeZFarther(previousChild))
                .Append(_subAnimations.MoveNodeY(previousChild, height)));
        }

        return sequence;
    }

    public override Tween AnimateChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> childChildList = new();

        for (int i = 0; i < childsToBe.Count; i++)
        {
            CardNode node = childsToBe[i];

            _cardManager.AddToTopLevel(node);

            foreach (CardNode childChild in node.Children)
            {
                _cardManager.AddToTopLevel(childChild, false);
                childChildList.Add(childChild);
            }            

            sequence.Join(
                DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(node, baseNode))
                .Append(_subAnimations.MoveNodeXToLeft(node))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.FanOutCard(node, i, childsToBe.Count, false))
            );
        }
        
        for (int i = childChildList.Count-1; i >= 0; i--)
        {
            CardNode node = childChildList[i];

            _height += node.GetNodeCount(CardTraversal.CONTEXT);

            sequence.Join(
                DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(node, baseNode))
                .Append(_subAnimations.MoveNodeXToLeft(node))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeY(node, _height)));
        }

        return sequence;
    }

    public override Tween MoveNewBaseNode(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Append(_subAnimations.LiftAndMoveChildToBase(activeNode, baseNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(activeNode, _height + 1));
    }
}