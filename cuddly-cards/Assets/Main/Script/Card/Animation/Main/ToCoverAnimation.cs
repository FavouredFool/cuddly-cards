using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using static CardInfo;

public class ToCoverAnimation : MainAnimation
{
    public ToCoverAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        List<CardNode> children = baseNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(child, _cardManager.RootNode))
                .Append(_subAnimations.MoveNodeXToLeft(child))
                .AppendInterval(2 * _waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeXToMiddle(child)));
        }

        return sequence;
    }

    public override Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, _cardManager.RootNode))
            .AppendInterval(2 * _horizontalTime + 2 * _waitTime)
            .Append(_subAnimations.MoveNodeXToMiddle(baseNode));
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

        foreach (CardNode node in animatingNodesBack)
        {
            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(node, _cardManager.RootNode))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeZNearer(node))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToMiddle(node)));
        }

        return sequence;
    }

    public override Tween RootAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode backNode = baseNode.Parent;
        CardNode rootNode = _cardManager.RootNode;

        List<CardNode> lowerTopMostCardsRoot = backNode.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

        foreach (CardNode node in lowerTopMostCardsRoot)
        {
            _cardManager.AddToTopLevel(node);
        }

        List<CardNode> animatingNodesRoot = lowerTopMostCardsRoot;
        animatingNodesRoot.Add(rootNode);

        foreach (CardNode node in animatingNodesRoot)
        {
            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(node, rootNode))
                .Append(_subAnimations.MoveNodeXToLeft(node))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeZNearer(node))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToMiddle(node)));
        }

        return sequence;
    }
}
