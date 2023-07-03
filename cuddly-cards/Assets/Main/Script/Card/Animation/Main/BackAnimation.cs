
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackAnimation : CardAnimation
{
    public BackAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;
        CardNode backToBe = activeNode.Parent;
        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> previousChilds = previousActiveNode.Children;

        CardNode discard = backToBe != null && backToBe != rootNode ? rootNode : null;

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            _cardManager.AddToTopLevelMainPile(newChild);


            if (previousActiveNode == newChild)
            {
                // ------------- PREVIOUS MAIN ----------------

                entireSequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.RaiseNodePileRelative(previousActiveNode, activeNode))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(_subAnimations.MoveBaseToChild(previousActiveNode, previousActiveNode))
                    .Append(_subAnimations.RaiseNodePileRelative(previousActiveNode, previousActiveNode)));

                // ------------- PREVIOUS CHILDREN ----------------
                for (int j = previousChilds.Count - 1; j >= 0; j--)
                {
                    CardNode oldChild = previousChilds[j];
                    _cardManager.AddToTopLevelMainPile(oldChild);

                    entireSequence.Join(DOTween.Sequence()
                        .Append(_subAnimations.RaiseNodePileRelative(oldChild, activeNode))
                        .Append(_subAnimations.MoveNodeToLeft(oldChild))
                        .AppendInterval(_waitTime)
                        .Append(_subAnimations.MoveBaseToChild(oldChild, previousActiveNode))
                        .Append(_subAnimations.RaiseNodePileRelative(oldChild, previousActiveNode)));  
                }

            }
            else
            {
                // ------------- NEW CHILDREN ----------------

                entireSequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.RaiseNodePileRelative(newChild, activeNode))
                    .Append(_subAnimations.MoveNodeNearer(newChild))
                    .AppendInterval(_waitTime)
                    .Append(_subAnimations.MoveBaseToChild(newChild, newChild))
                    .Append(_subAnimations.LowerNodePile(newChild)));
            }
        }

        // ------------- NEW MAIN ----------------

        _cardManager.AddToTopLevelMainPile(activeNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.LiftNodePile(activeNode))
            .Append(_subAnimations.MoveNodeNearer(activeNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.LowerNodePile(activeNode)));


        if (discard != null)
        {
            _cardManager.AddToTopLevelMainPile(discard);
            _cardManager.AddToTopLevelMainPile(backToBe);

            // ------------- DISCARD ----------------

            int discardHeight = discard.GetNodeCount(CardTraversal.BODY);

            List<CardNode> lowerTopMostCardsRoot = backToBe.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevelMainPile(node);
            }

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime + _horizontalTime)
                .Append(_subAnimations.RaiseNodeToHeight(discard, discardHeight)));

            // ------------- BackToBe ----------------


            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeToLeft(backToBe))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.LowerNodePile(backToBe)));
        }
        else if (backToBe != null)
        {
            // ------------- BackToBe ----------------

            _cardManager.AddToTopLevelMainPile(backToBe);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeToLeft(backToBe)));
        }

        return entireSequence;
    }

}
