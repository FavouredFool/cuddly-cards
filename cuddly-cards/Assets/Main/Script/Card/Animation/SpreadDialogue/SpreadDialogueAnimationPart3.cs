
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class SpreadDialogueAnimationPart3 : MainAnimation
{
    public SpreadDialogueAnimationPart3(CardManager cardManager) : base(cardManager) { }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        List<CardNode> children = baseNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];

            sequence.Join(DOTween.Sequence()
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.MoveOutChildFromBase(child))
                .Append(_subAnimations.MoveNodeYLowerPile(child)));
        }

        return sequence;
    }

    public override Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .AppendInterval(2 * _horizontalTime + _waitTime)
            .Append(_subAnimations.MoveNodeYLowerPile(baseNode));
    }

    public override Tween BackAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode backNode = baseNode.Parent;

        sequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeZFarther(backNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(backNode, backNode.GetNodeCount(CardTraversal.BODY))));

        return sequence;
    }

    public override Tween RootAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode backNode = baseNode.Parent;
        CardNode rootNode = _cardManager.RootNode;

        if (rootNode != backNode)
        {
            List<CardNode> lowerTopMostCardsBack = baseNode.GetTopNodesBelowNodeInPile(backNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsBack)
            {
                _cardManager.AddToTopLevel(node);
            }

            foreach (CardNode node in lowerTopMostCardsBack)
            {
                int height = node.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT);
                int backNodeHeight = backNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT);

                if (height > backNodeHeight)
                {
                    height -= backNodeHeight;
                }

                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeZFarther(node))
                    .AppendInterval(_waitTime)
                    .Append(_subAnimations.MoveNodeXToRight(node))
                    .Append(_subAnimations.MoveNodeY(node, height)));
            }

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeZFarther(rootNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToRight(rootNode))
                .Append(_subAnimations.MoveNodeY(rootNode, rootNode.GetNodeCount(CardTraversal.CONTEXT) - backNode.GetNodeCount(CardTraversal.CONTEXT))));
        }

        return sequence;
    }
}
