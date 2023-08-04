using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using static CardInfo;

public class ToCoverAnimation : CardAnimation
{
    public ToCoverAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;

        CardNode backNode = baseNode.Parent;

        // -------------- CHILDREN ---------------------

        List<CardNode> children = baseNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.MoveNodeYLiftPile(child, rootNode))
                .Append(_subAnimations.MoveNodeXToLeft(child))
                .AppendInterval(2 * _waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeXToMiddle(child)));
        }



        // -------------- MAIN ---------------------
        
        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, rootNode))
            .AppendInterval(2 * _horizontalTime + 2 * _waitTime)
            .Append(_subAnimations.MoveNodeXToMiddle(baseNode)));
        
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
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToMiddle(node)));
        }


        // -------------- ROOT ---------------------
        
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
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToMiddle(node)));
        }
        
        return entireSequence;
    }

}
