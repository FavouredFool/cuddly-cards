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
            _cardManager.AddToTopLevelMainPile(child);

            entireSequence.Join(DOTween.Sequence()
                .Append(_subAnimations.RaiseNodePileRelative(child, rootNode))
                .Append(_subAnimations.MoveNodeToLeft(child))
                .AppendInterval(2 * _waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeToMiddle(child)));
        }



        // -------------- MAIN ---------------------
        
        _cardManager.AddToTopLevelMainPile(baseNode);
        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.RaiseNodePileRelative(baseNode, rootNode))
            .AppendInterval(2 * _horizontalTime + 2 * _waitTime)
            .Append(_subAnimations.MoveNodeToMiddle(baseNode)));
        
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
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeToMiddle(node)));
        }


        // -------------- ROOT ---------------------
        
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
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeToMiddle(node)));
        }
        
        return entireSequence;
    }

}
