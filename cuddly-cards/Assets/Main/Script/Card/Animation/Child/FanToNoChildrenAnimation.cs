
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FanToNoChildrenAnimation : ChildParentAnimation
{
    public FanToNoChildrenAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween MoveNewBaseNode(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Append(_subAnimations.FanInCard(activeNode, baseNode, false, false))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(activeNode, activeNode.GetNodeCount(CardTraversal.CONTEXT)));
    }

    

    public override Tween AnimateOldChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(AnimateOldChildrenChildren(activeNode, baseNode));

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
                .Append(_subAnimations.FanInCard(previousChild, baseNode, false, false))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeZFarther(previousChild))
                .Append(_subAnimations.MoveNodeY(previousChild, height)));
        }

        return sequence;
    }

    public Tween AnimateOldChildrenChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (CardNode child in baseNode.Children)
        {
            if (child == activeNode)
            {
                continue;
            }

            foreach (CardNode childChild in child.Children)
            {
                _cardManager.AddToTopLevel(childChild);

                int height = childChild.GetNodeCountUpToNodeInPile(baseNode, CardTraversal.CONTEXT);

                if (height > activeNode.GetNodeCountBelowNodeInPile(baseNode, CardTraversal.CONTEXT))
                {
                    height -= activeNode.GetNodeCount(CardTraversal.CONTEXT);
                }

                // Move to correct size, then follow what the oldchildren do
                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeY(childChild, childChild.GetNodeCountUpToNodeInPile(baseNode, CardTraversal.CONTEXT)))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(_subAnimations.MoveNodeZFarther(childChild))
                    .Append(_subAnimations.MoveNodeY(childChild, height))
                    );

            }
        }
        
        return sequence;
    }

    public override Tween AnimateChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (CardNode child in activeNode.Children)
        {
            _cardManager.AddToTopLevel(child);

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeY(child, child.GetNodeCountUpToNodeInPile(baseNode, CardTraversal.CONTEXT)))
                .AppendInterval(_horizontalTime + _waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeY(child, child.GetNodeCountUpToNodeInPile(activeNode, CardTraversal.CONTEXT)))
                );
        }

        return sequence;
    }

}
