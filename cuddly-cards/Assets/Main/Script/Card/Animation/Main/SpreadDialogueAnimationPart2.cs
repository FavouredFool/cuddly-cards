
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class SpreadDialogueAnimationPart2 : CardAnimation
{
    public SpreadDialogueAnimationPart2(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode rootNode = _cardManager.RootNode;

        CardNode backNode = baseNode.Parent;

        CardNode parentTalkNode = _cardManager.GetCardNodeFromID(activeNode.Context.TalkID);

        // -------------- ACTIVE NODE ------------------
        
        _cardManager.AddToTopLevelMainPile(activeNode);

        int activeHeight = parentTalkNode.GetNodeCountBelowNodeInPile(rootNode, CardTraversal.CONTEXT) + activeNode.GetNodeCount(CardTraversal.CONTEXT);

        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeY(activeNode, activeHeight))
            .Append(_subAnimations.MoveNodeXToLeft(activeNode)));


        // -------------- ROOT ---------------------

        List<CardNode> lowerTopMostCards = parentTalkNode.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

        foreach (CardNode node in lowerTopMostCards)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }
        
        _cardManager.AddToTopLevelMainPile(rootNode);

        int rootHeight = rootNode.GetNodeCount(CardTraversal.CONTEXT) + activeNode.GetNodeCount(CardTraversal.CONTEXT);

        entireSequence.Join(_subAnimations.MoveNodeY(rootNode, rootHeight));

        return entireSequence;
    }
}
