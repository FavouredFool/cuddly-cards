
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackFanToDefaultAnimation : BackParentAnimation
{
    public BackFanToDefaultAnimation(CardManager cardManager) : base(cardManager)
    {
    }

    public override Tween SetActiveNode(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(activeNode, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(activeNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeYLowerPile(activeNode));
    }

    public override Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(SetBaseNode(activeNode, baseNode));
        sequence.Join(SetBaseNodeChildren(activeNode, baseNode));
        sequence.Join(SetBaseNodeChildrenChildren(activeNode, baseNode));
        sequence.Join(SetOtherChildren(activeNode, baseNode));

        return sequence;
    }

    public Tween SetBaseNode(CardNode activeNode, CardNode baseNode)
    {
        // BaseNode is being turned into a child.
        _cardManager.AddToTopLevel(baseNode);

        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, activeNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_subAnimations.MoveNodeXToChild(baseNode, baseNode))
            .Append(_subAnimations.MoveNodeY(baseNode, baseNode.GetNodeCount(CardTraversal.CONTEXT)));
    }

    public Tween SetBaseNodeChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // The children of the previous baseNode

        foreach (CardNode child in baseNode.Children)
        {
            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.FanInCard(child, activeNode, false, false))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToChild(child, baseNode))
                .Append(_subAnimations.MoveNodeY(child, child.GetNodeCountUpToNodeInPile(baseNode, CardTraversal.CONTEXT)))
                );
        }

        return sequence;
    }

    public Tween SetBaseNodeChildrenChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // the children of the children that are initially below the baseNode

        foreach (CardNode child in baseNode.Children)
        {
            foreach (CardNode childChild in child.Children)
            {
                _cardManager.AddToTopLevel(childChild);

                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(childChild, activeNode))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(_subAnimations.MoveNodeXToChild(childChild, baseNode))
                    .Append(_subAnimations.MoveNodeY(childChild, childChild.GetNodeCountUpToNodeInPile(baseNode, CardTraversal.CONTEXT)))
                    );
            }
        }

        return sequence;
    }

    public Tween SetOtherChildren(CardNode activeNode, CardNode baseNode)
    {

        // Same as DefaultToDefault -> Can be combined
        Sequence sequence = DOTween.Sequence();

        foreach (CardNode child in activeNode.Children)
        {
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
            .Append(_subAnimations.MoveOutChildFromBase(child))
            );
        }

        return sequence;
    }

}
