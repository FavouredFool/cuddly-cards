
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ChildAnimation : CardAnimation
{
    public ChildAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;

        CardNode discardToBe = previousActiveNode.Parent;
        CardNode discard = discardToBe != null && discardToBe != rootNode ? rootNode : null;

        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> previousChilds = previousActiveNode.Children;


        // ------------- CHILDS TO BE ----------------

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            _cardManager.AddToTopLevelMainPile(childsToBe[i]);

            entireSequence.Join(
                DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(childsToBe[i], previousActiveNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.FanOutChildFromBase(childsToBe[i]))
            );
        }

        // ------------- MAIN TO BE ----------------

        _cardManager.AddToTopLevelMainPile(activeNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.LiftAndMoveChildToBase(activeNode, previousActiveNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.LowerNodePile(activeNode)));


        // ------------- Previous Children ----------------

        int height = 0;

        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = previousChilds[i];

            if (previousChild == activeNode)
            {
                continue;
            }

            _cardManager.AddToTopLevelMainPile(previousChild);

            height += previousChild.GetNodeCount(CardTraversal.CONTEXT);

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(previousChild, previousActiveNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeFarther(previousChild))
                .Append(_subAnimations.RaiseNodeToHeight(previousChild, height)));
        }

        // ------------- BackToBe ----------------

        _cardManager.AddToTopLevelMainPile(previousActiveNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.LiftNodePile(previousActiveNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_subAnimations.MoveNodeFarther(previousActiveNode))
            .Append(_subAnimations.RaiseNodeToHeight(previousActiveNode, previousActiveNode.GetNodeCount(CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT))));


        // ------------- Discard & DiscardToBe ----------------

        if (discard != null)
        {
            _cardManager.AddToTopLevelMainPile(discard);
            _cardManager.AddToTopLevelMainPile(discardToBe);

            // height needs to be calculated before the deck is split in two, because otherwise new top-levels would be overlooked (this is a bit ugly)
            int discardHeight = discard.GetNodeCount(CardTraversal.BODY) + discardToBe.GetNodeCount(CardTraversal.BODY);
            int discardToBeHeight = discardToBe.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.BODY);

            List<CardNode> lowerTopMostCardsRoot = discardToBe.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevelMainPile(node);
            }

            entireSequence.Join(_subAnimations.RaiseNodeToHeight(rootNode, discardHeight));

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.RaiseNodeToHeight(discardToBe, discardToBeHeight))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeToRight(discardToBe)));
        }
        else if (discardToBe != null)
        {
            _cardManager.AddToTopLevelMainPile(discardToBe);
            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeToRight(discardToBe)));
        }

        return entireSequence;
    }
}
