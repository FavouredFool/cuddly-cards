
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public abstract class ChildParentAnimation : CardAnimation
{
    public ChildParentAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;

        CardNode discardToBe = baseNode.Parent;
        CardNode discard = discardToBe != null && discardToBe != rootNode ? rootNode : null;

        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> previousChilds = baseNode.Children;


        // ------------- CHILDS TO BE ----------------

        entireSequence.Join(AnimateChildren(activeNode, baseNode));
        

        // ------------- MAIN TO BE ----------------

        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.LiftAndMoveChildToBase(activeNode, baseNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeYLowerPile(activeNode)));


        // ------------- Previous Children ----------------

        int height = 0;

        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = previousChilds[i];

            if (previousChild == activeNode)
            {
                continue;
            }

            height += previousChild.GetNodeCount(CardTraversal.CONTEXT);

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(previousChild, baseNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeZFarther(previousChild))
                .Append(_subAnimations.MoveNodeY(previousChild, height)));
        }

        // ------------- BackToBe ----------------

        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_subAnimations.MoveNodeZFarther(baseNode))
            .Append(_subAnimations.MoveNodeY(baseNode, baseNode.GetNodeCount(CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT))));


        // ------------- Discard & DiscardToBe ----------------

        if (discard != null)
        {
            // height needs to be calculated before the deck is split in two, because otherwise new top-levels would be overlooked (this is a bit ugly)
            int discardHeight = discard.GetNodeCount(CardTraversal.BODY) + discardToBe.GetNodeCount(CardTraversal.BODY);
            int discardToBeHeight = discardToBe.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.BODY);

            List<CardNode> lowerTopMostCardsRoot = discardToBe.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevel(node);
            }

            entireSequence.Join(_subAnimations.MoveNodeY(rootNode, discardHeight));

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeY(discardToBe, discardToBeHeight))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeXToRight(discardToBe)));
        }
        else if (discardToBe != null)
        {
            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeXToRight(discardToBe)));
        }

        return entireSequence;
    }

    public abstract Tween AnimateChildren(CardNode activeNode, CardNode baseNode);
}
