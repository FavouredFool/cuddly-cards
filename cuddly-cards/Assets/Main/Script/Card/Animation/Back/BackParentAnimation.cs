
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public abstract class BackParentAnimation : CardAnimation
{
    public BackParentAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;
        CardNode backToBe = activeNode.Parent;
        List<CardNode> childsToBe = activeNode.Children;
        List<CardNode> previousChilds = baseNode.Children;

        CardNode discard = backToBe != null && backToBe != rootNode ? rootNode : null;


        // ------------- CHILDREN AND BASE ----------------

        entireSequence.Join(AnimateChildrenAndBase(activeNode, baseNode));

        // ------------- NEW MAIN ----------------

        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(activeNode, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(activeNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeYLowerPile(activeNode)));


        if (discard != null)
        {
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

            _cardManager.AddToTopLevelMainPile(backToBe);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(backToBe))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeYLowerPile(backToBe)));
        }
        else if (backToBe != null)
        {
            // ------------- BackToBe ----------------

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(backToBe)));
        }

        return entireSequence;
    }

    public abstract Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode);
}
