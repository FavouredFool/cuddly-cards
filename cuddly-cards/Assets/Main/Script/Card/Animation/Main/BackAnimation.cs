
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackAnimation : CardAnimation
{
    public BackAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;
        CardNode backToBe = activeNode.Parent;
        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> previousChilds = baseNode.Children;

        CardNode discard = backToBe != null && backToBe != rootNode ? rootNode : null;

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            _cardManager.AddToTopLevelMainPile(newChild);


            if (baseNode == newChild)
            {
                // ------------- PREVIOUS MAIN ----------------

                entireSequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(baseNode, activeNode))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(_subAnimations.MoveNodeXToChild(baseNode, baseNode))
                    .Append(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode)));

                // ------------- PREVIOUS CHILDREN ----------------
                for (int j = previousChilds.Count - 1; j >= 0; j--)
                {
                    CardNode oldChild = previousChilds[j];
                    _cardManager.AddToTopLevelMainPile(oldChild);

                    entireSequence.Join(DOTween.Sequence()
                        .Append(_subAnimations.MoveNodeYLiftPile(oldChild, activeNode))
                        .Append(_subAnimations.MoveNodeXToLeft(oldChild))
                        .AppendInterval(_waitTime)
                        .Append(_subAnimations.MoveNodeXToChild(oldChild, baseNode))
                        .Append(_subAnimations.MoveNodeYLiftPile(oldChild, baseNode)));  
                }

            }
            else
            {
                // ------------- NEW CHILDREN ----------------

                entireSequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(newChild, activeNode))
                    .Append(_subAnimations.MoveNodeZNearer(newChild))
                    .AppendInterval(_waitTime)
                    .Append(_subAnimations.MoveNodeXToChild(newChild, newChild))
                    .Append(_subAnimations.MoveNodeYLowerPile(newChild)));
            }
        }

        // ------------- NEW MAIN ----------------

        _cardManager.AddToTopLevelMainPile(activeNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(activeNode, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(activeNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeYLowerPile(activeNode)));


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
                .Append(_subAnimations.MoveNodeY(discard, discardHeight)));

            // ------------- BackToBe ----------------


            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(backToBe))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeYLowerPile(backToBe)));
        }
        else if (backToBe != null)
        {
            // ------------- BackToBe ----------------

            _cardManager.AddToTopLevelMainPile(backToBe);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(backToBe)));
        }

        return entireSequence;
    }

}
