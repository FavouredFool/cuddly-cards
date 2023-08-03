
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FanDialogueAnimation : CardAnimation
{
    public FanDialogueAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;

        CardNode discardToBe = baseNode.Parent;
        CardNode discard = discardToBe != null && discardToBe != rootNode ? rootNode : null;

        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> previousChilds = baseNode.Children;


        // ------------- CHILDS TO BE ----------------

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode node = childsToBe[i];

            _cardManager.AddToTopLevelMainPile(childsToBe[i]);

            entireSequence.Join(
                DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeToLeft(node))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.FanOutCards(node, i, childsToBe.Count, false))
            );
        }

        // ------------- MAIN TO BE ----------------

        _cardManager.AddToTopLevelMainPile(activeNode);
        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_subAnimations.MoveNodeToLeft(activeNode))
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
                .Append(_subAnimations.LiftAndMoveChildToBase(previousChild, baseNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeFarther(previousChild))
                .Append(_subAnimations.RaiseNodeToHeight(previousChild, height)));
        }

        // ------------- BackToBe ----------------

        _cardManager.AddToTopLevelMainPile(baseNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.LiftNodePile(baseNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_subAnimations.MoveNodeFarther(baseNode))
            .Append(_subAnimations.RaiseNodeToHeight(baseNode, baseNode.GetNodeCount(CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT))));


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
