
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public abstract class ChildParentAnimation : MainAnimation
{
    public ChildParentAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
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

    public override Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_subAnimations.MoveNodeZFarther(baseNode))
            .Append(_subAnimations.MoveNodeY(baseNode, baseNode.GetNodeCount(CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT)));
    }

    public override Tween BackAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;
        CardNode backNode = baseNode.Parent;

        if (backNode == null)
        {
            return sequence;
        }

        if (backNode != rootNode)
        {
            int discardToBeHeight = backNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.BODY);

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeY(backNode, discardToBeHeight))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeXToRight(backNode)));
        }
        else
        {
            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeXToRight(backNode)));
        }

        return sequence;
    }

    public override Tween RootAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;
        CardNode backNode = baseNode.Parent;

        if (backNode == null)
        {
            return sequence;
        }

        if (backNode == rootNode)
        {
            return sequence;
        }

        int discardHeight = rootNode.GetNodeCount(CardTraversal.BODY) + backNode.GetNodeCount(CardTraversal.BODY);
            
        foreach (CardNode node in backNode.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY))
        {
            _cardManager.AddToTopLevel(node);
        }

        sequence.Join(_subAnimations.MoveNodeY(rootNode, discardHeight));
  
        return sequence;
    }

    public override Tween OtherAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // newChildren
        sequence.Join(AnimateChildren(activeNode, baseNode));

        // newActiveNode
        sequence.Join(MoveBaseNode(activeNode, baseNode));
        
        return sequence;
    }

    public abstract Tween AnimateChildren(CardNode activeNode, CardNode baseNode);

    public virtual Tween MoveBaseNode(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Append(_subAnimations.LiftAndMoveChildToBase(activeNode, baseNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeYLowerPile(activeNode));
    }
}
