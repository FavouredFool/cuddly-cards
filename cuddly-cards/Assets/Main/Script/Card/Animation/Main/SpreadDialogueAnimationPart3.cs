
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
                .Append(_subAnimations.FanOutChildFromBase(child))
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

        List<CardNode> lowerTopMostCardsBack = baseNode.GetTopNodesBelowNodeInPile(backNode, CardTraversal.BODY);

        foreach (CardNode node in lowerTopMostCardsBack)
        {
            _cardManager.AddToTopLevel(node);
        }

        List<CardNode> animatingNodesBack = lowerTopMostCardsBack;
        animatingNodesBack.Add(backNode);

        // TODO FIX THIS WHEN THE JSON EDITOR IS DONE

        /*
        foreach (CardNode node in animatingNodesBack)
        {
            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeFarther(node))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_subAnimations.RaiseNodeToHeight(node, node.GetNodeCountUpToNodeInPile(backNode, CardTraversal.BODY))));
        }
        */

        return sequence;
    }

    public override Tween RootAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode backNode = baseNode.Parent;
        CardNode rootNode = _cardManager.RootNode;

        if (rootNode != backNode)
        {
            // FIX THIS WHEN THE JSON EDITOR IS DONE

            sequence.Append(_subAnimations.MoveNodeY(rootNode, 300));
        }

        return sequence;
    }
}
