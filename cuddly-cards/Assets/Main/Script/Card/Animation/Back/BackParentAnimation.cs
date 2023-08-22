
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public abstract class BackParentAnimation : MainAnimation
{
    public BackParentAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }

    public override Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        return SetActiveNode(activeNode, baseNode);
    }

    public override Tween BackAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }

    public override Tween RootAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode backToBe = activeNode.Parent;
        CardNode rootNode = _cardManager.RootNode;

        if (backToBe == null)
        {
            return sequence;
        }

        if (backToBe != rootNode)
        {
            int rootHeight = rootNode.GetNodeCount(CardTraversal.CONTEXT) - backToBe.GetNodeCount(CardTraversal.CONTEXT);

            List<CardNode> lowerTopMostCardsRoot = backToBe.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevel(node);
            }

            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeY(rootNode, rootHeight)));


            _cardManager.AddToTopLevel(backToBe);

            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(backToBe))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeYLowerPile(backToBe)));
        }
        else
        {
            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToLeft(backToBe)));
        }

        return sequence;
    }

    public override Tween OtherAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(AnimateChildrenAndBase(activeNode, baseNode));

        return sequence;
    }

    public abstract Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode);

    public abstract Tween SetActiveNode(CardNode activeNode, CardNode baseNode);
}
