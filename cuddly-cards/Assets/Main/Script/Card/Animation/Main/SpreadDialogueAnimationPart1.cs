
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

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(2 * _waitTime + 2 * _horizontalTime + _verticalTime)
            .Append(_subAnimations.MoveNodeXToMiddle(activeNode))
            .Append(_subAnimations.MoveNodeYLowerPile(activeNode))
            );


        // -------------- CHILDREN ---------------------

        List<CardNode> children = baseNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];
            
            entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(child, rootNode))
            .Append(_subAnimations.MoveNodeXToLeft(child))
            .AppendInterval(2 * _waitTime + 3 * _horizontalTime + _verticalTime));
        }

        for (int i = activeNode.Parent.Children.IndexOf(activeNode) - 1; i >= 0; i--)
        {
            CardNode otherChild = activeNode.Parent.Children[i];
            int height = otherChild.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT);

            // All children that are on the same level but physically above
            entireSequence.Join(DOTween.Sequence()
            .AppendInterval(2 * _waitTime + 3 * _horizontalTime + _verticalTime)
            .Append(_subAnimations.MoveNodeY(otherChild, height)));
        }


        // -------------- MAIN ---------------------

        int baseHeight = baseNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT);

        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, rootNode))
            .AppendInterval(3 * _horizontalTime + 2 * _waitTime)
            .Append(_subAnimations.MoveNodeY(baseNode, baseHeight)));


        // -------------- BACK ---------------------

        List<CardNode> lowerTopMostCardsBack = baseNode.GetTopNodesBelowNodeInPile(backNode, CardTraversal.BODY);

        foreach (CardNode node in lowerTopMostCardsBack)
        {
            _cardManager.AddToTopLevel(node);
        }

        List<CardNode> animatingNodesBack = lowerTopMostCardsBack;
        animatingNodesBack.Add(backNode);

        foreach (CardNode node in animatingNodesBack)
        {
            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(node, rootNode))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(_subAnimations.MoveNodeZNearer(node))
                .AppendInterval(_waitTime + _horizontalTime + _verticalTime));
        }

        int backHeight = backNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT);

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(2 * _waitTime + 3 * _horizontalTime + _verticalTime)
            .Append(_subAnimations.MoveNodeY(backNode, backHeight)));


        // -------------- ROOT ---------------------

        if (rootNode != backNode)
        {
            List<CardNode> lowerTopMostCardsRoot = backNode.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevel(node);
            }

            List<CardNode> animatingNodesRoot = lowerTopMostCardsRoot;
            animatingNodesRoot.Add(rootNode);

            foreach (CardNode node in animatingNodesRoot)
            {
                entireSequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(node, rootNode))
                    .Append(_subAnimations.MoveNodeXToLeft(node))
                    .AppendInterval(_waitTime)
                    .Append(_subAnimations.MoveNodeZNearer(node))
                    .AppendInterval(_waitTime + _horizontalTime + _verticalTime));
            }

            int rootHeight = rootNode.GetNodeCountUpToNodeInPile(rootNode, CardTraversal.CONTEXT) - activeNode.GetNodeCount(CardTraversal.CONTEXT);

            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(2 * _waitTime + 3 * _horizontalTime + _verticalTime)
                .Append(_subAnimations.MoveNodeY(rootNode, rootHeight)));
        }

        return entireSequence;
    }
}
