
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class SpreadDialogueAnimationPart2 : FreeAnimationParent
{
    public SpreadDialogueAnimationPart2(CardManager cardManager) : base(cardManager) { }

    public override Tween RootAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode parentTalkNode = _cardManager.GetCardNodeFromID(activeNode.Context.DesiredTalkID);
        CardNode rootNode = _cardManager.RootNode;

        _cardManager.AddToTopLevel(rootNode);

        List<CardNode> lowerTopMostCards = parentTalkNode.GetTopNodesBelowNodeInPile(rootNode, CardTraversal.CONTEXT);

        foreach (CardNode node in lowerTopMostCards)
        {
            _cardManager.AddToTopLevel(node);
        }

        int rootHeight = rootNode.GetNodeCount(CardTraversal.CONTEXT) + activeNode.GetNodeCount(CardTraversal.CONTEXT);

        sequence.Join(_subAnimations.MoveNodeY(rootNode, rootHeight));

        return sequence;
    }

    public override Tween OtherAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        CardNode parentTalkNode = _cardManager.GetCardNodeFromID(activeNode.Context.DesiredTalkID);

        int activeHeight = parentTalkNode.GetNodeCountBelowNodeInPile(_cardManager.RootNode, CardTraversal.CONTEXT) + activeNode.GetNodeCount(CardTraversal.CONTEXT);

        sequence
            .Append(_subAnimations.MoveNodeY(activeNode, activeHeight))
            .Append(_subAnimations.MoveNodeXToLeft(activeNode));

        return sequence;
    }
}
