
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackDefaultToFanAnimation : BackParentAnimation
{
    public BackDefaultToFanAnimation(CardManager cardManager) : base(cardManager)
    {
    }


    public override Tween SetActiveNode(CardNode activeNode, CardNode baseNode)
    {
        int height = activeNode.GetNodeCount(CardTraversal.CONTEXT) - activeNode.Children.Count;

        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(activeNode, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(activeNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(activeNode, height));
    }

    public override Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(SetBaseNode(activeNode, baseNode));
        sequence.Join(SetBaseNodeChildren(activeNode, baseNode));
        sequence.Join(SetOtherChildren(activeNode, baseNode));
        sequence.Join(SetOtherChildrenChildren(activeNode, baseNode));

        return sequence;
    }

    public Tween SetBaseNode(CardNode activeNode, CardNode baseNode)
    {
        // BaseNode is being turned into a child.
        _cardManager.AddToTopLevel(baseNode);

        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, activeNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_subAnimations.FanOutCard(baseNode, baseNode.Parent.Children.IndexOf(baseNode), baseNode.Parent.Children.Count, false));
    }

    public Tween SetBaseNodeChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // The children of the previous baseNode

        foreach (CardNode child in baseNode.Children)
        {
            // The height of all the children below them, but only the children. So you take cards below and subtract the amount of other children to the left of basenode
            int countUp = child.GetNodeCountUpToNodeInPile(activeNode, CardTraversal.CONTEXT) + 1;
            int subtract = baseNode.Parent.Children.Count - baseNode.Parent.Children.IndexOf(baseNode);

            int height = countUp - subtract;

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(child, activeNode))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeY(child, height))
                );
        }

        return sequence;
    }

    public Tween SetOtherChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < activeNode.Children.Count; i++)
        {
            CardNode child = activeNode.Children[i];

            if (child == baseNode)
            {
                continue;
            }

            // Other children
            _cardManager.AddToTopLevel(child);

            sequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(child, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(child))
            .AppendInterval(_waitTime)
            .Append(_subAnimations.FanOutCard(child, i, activeNode.Children.Count, false))
            );
        }

        return sequence;
    }

    public Tween SetOtherChildrenChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // The children of the previous baseNode

        foreach (CardNode child in activeNode.Children)
        {
            if (child == baseNode)
            {
                continue;
            }

            foreach (CardNode childChild in child.Children)
            {
                _cardManager.AddToTopLevel(childChild);

                // The height of all the children below them, but only the children. So you take cards below and subtract the amount of other children to the left of basenode
                int countUp = childChild.GetNodeCountUpToNodeInPile(activeNode, CardTraversal.CONTEXT);
                int subtract = child.Parent.Children.Count - (child.Parent.Children.IndexOf(child) + 1);

                int height = countUp - subtract;

                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(childChild, activeNode))
                    .Append(_subAnimations.MoveNodeZNearer(childChild))
                    .AppendInterval(_waitTime + _horizontalTime)
                    .Append(_subAnimations.MoveNodeY(childChild, height))
                    );
            }
        }

        return sequence;
    }
}
