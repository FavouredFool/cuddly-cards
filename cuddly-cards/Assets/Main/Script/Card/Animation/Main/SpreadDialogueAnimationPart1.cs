
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class SpreadDialogueAnimationPart1 : CardAnimation
{
    public SpreadDialogueAnimationPart1(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;

        CardNode backNode = baseNode.Parent;


        // -------------- ACTIVE NODE ------------------

        _cardManager.AddToTopLevelMainPile(activeNode);

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(2 * _waitTime + 2 * _horizontalTime + _verticalTime)
            .Append(_subAnimations.MoveNodeToMiddle(activeNode))
            .Append(_subAnimations.LowerNodePile(activeNode))
            );


        // -------------- CHILDREN ---------------------

        List<CardNode> children = baseNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];
            _cardManager.AddToTopLevelMainPile(child);
            
            entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.RaiseNodePileRelative(child, rootNode))
            .Append(_subAnimations.MoveNodeToLeft(child))
            .AppendInterval(2 * _waitTime + 3 * _horizontalTime + _verticalTime));
        }

        for (int i = activeNode.Parent.Children.IndexOf(activeNode) - 1; i >= 0; i--)
        {
            CardNode otherChild = activeNode.Parent.Children[i];
            int height = otherChild.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT);

            // All children that are on the same level but physically above
            entireSequence.Join(DOTween.Sequence()
            .AppendInterval(2 * _waitTime + 3 * _horizontalTime + _verticalTime)
            .Append(_subAnimations.RaiseNodeToHeight(otherChild, height)));
        }


        // -------------- MAIN ---------------------

        int baseHeight = baseNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT);

        _cardManager.AddToTopLevelMainPile(baseNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.RaiseNodePileRelative(baseNode, rootNode))
            .AppendInterval(3 * _horizontalTime + 2 * _waitTime)
            .Append(_subAnimations.RaiseNodeToHeight(baseNode, baseHeight)));


        // -------------- BACK ---------------------

        _cardManager.AddToTopLevelMainPile(backNode);

        List<CardNode> lowerTopMostCardsBack = baseNode.GetTopNodesBelowNodeInPile(backNode, CardTraversal.BODY);

        foreach (CardNode node in lowerTopMostCardsBack)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        List<CardNode> animatingNodesBack = lowerTopMostCardsBack;
        animatingNodesBack.Add(backNode);

        foreach (CardNode node in animatingNodesBack)
        {
            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.RaiseNodePileRelative(node, rootNode))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeNearer(node))
                .AppendInterval(_waitTime + _horizontalTime + _verticalTime));
        }

        int backHeight = backNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT);

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(2 * _waitTime + 3 * _horizontalTime + _verticalTime)
            .Append(_subAnimations.RaiseNodeToHeight(backNode, backHeight)));


        // -------------- ROOT ---------------------

        if (rootNode != backNode)
        {
            _cardManager.AddToTopLevelMainPile(rootNode);
            List<CardNode> lowerTopMostCardsRoot = backNode.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevelMainPile(node);
            }

            List<CardNode> animatingNodesRoot = lowerTopMostCardsRoot;
            animatingNodesRoot.Add(rootNode);

            foreach (CardNode node in animatingNodesRoot)
            {
                entireSequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.RaiseNodePileRelative(node, rootNode))
                    .Append(_subAnimations.MoveNodeToLeft(node))
                    .AppendInterval(_waitTime)
                    .Append(_subAnimations.MoveNodeNearer(node))
                    .AppendInterval(_waitTime + _horizontalTime + _verticalTime));
            }

            int rootHeight = rootNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(2 * _waitTime + 3 * _horizontalTime + _verticalTime)
                .Append(_subAnimations.RaiseNodeToHeight(rootNode, rootHeight)));
        }

        return entireSequence;
    }
}