
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class SpreadDialogueAnimationPart3 : CardAnimation
{
    public SpreadDialogueAnimationPart3(CardManager cardManager) : base(cardManager) { }

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
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.FanOutChildFromBase(child))
                .Append(_subAnimations.MoveNodeYLowerPile(child)));
        }


        // -------------- MAIN ---------------------

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(2*_horizontalTime + _waitTime)
            .Append(_subAnimations.MoveNodeYLowerPile(baseNode)));


        // -------------- BACK ---------------------

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


        // -------------- ROOT ---------------------

        if (rootNode != backNode)
        {
            // FIX THIS WHEN THE JSON EDITOR IS DONE

            entireSequence.Append(_subAnimations.MoveNodeY(rootNode, 300));
        }

        return entireSequence;
    }
}
