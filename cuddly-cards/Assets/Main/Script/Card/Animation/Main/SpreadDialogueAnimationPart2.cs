
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
        
        int activeHeight = parentTalkNode.GetNodeCountBelowNodeInPile(rootNode, CardTraversal.CONTEXT) + activeNode.GetNodeCount(CardTraversal.CONTEXT);

        entireSequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeY(activeNode, activeHeight))
            .Append(_subAnimations.MoveNodeXToLeft(activeNode)));


        // -------------- ROOT ---------------------

        List<CardNode> lowerTopMostCards = parentTalkNode.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.BODY);

        foreach (CardNode node in lowerTopMostCards)
        {
            // Braucht man die noch, wenn man sie in Part1 bereits toplevel gemacht hat? Ist so'ne Dependency überhaupt OK? Vllt. pro Animation top-level resetten TODO
            _cardManager.AddToTopLevel(node);
        }

        int rootHeight = rootNode.GetNodeCount(CardTraversal.CONTEXT) + activeNode.GetNodeCount(CardTraversal.CONTEXT);

        entireSequence.Join(_subAnimations.MoveNodeY(rootNode, rootHeight));

        return entireSequence;
    }
}
