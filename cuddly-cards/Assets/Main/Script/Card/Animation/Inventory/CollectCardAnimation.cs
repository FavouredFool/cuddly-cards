
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class CollectCardAnimation : InventoryAnimation
{
    public CollectCardAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode parentNode = activeNode.Parent;
        CardType cardType = activeNode.Context.GetCardType();

        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        // activeNode vertical nach oben, relativ dem höchsten stack rechts der Node.

        int maxHeight = 0;

        for(int i = parentNode.Children.IndexOf(activeNode) + 1; i < parentNode.Children.Count; i++)
        {
            int tempHeight = parentNode.Children[i].GetNodeCount(CardTraversal.CONTEXT);

            if (tempHeight > maxHeight)
            {
                maxHeight = tempHeight;
            }
        }

        maxHeight += 1;


        int finalHeight = 1;

        switch (cardType)
        {
            case CardType.DIALOGUE:
                finalHeight = _cardInventory.GetInventoryNode()[1].GetNodeCount(CardTraversal.CONTEXT) + 1;
                break;
            case CardType.KEY:
                finalHeight = 1;
                break;
        }

        entireSequence.Join(
            DOTween.Sequence()
                .Append(_tweenYFunc(activeNode, maxHeight))
                .AppendInterval(_waitTime)
                .Append(_tweenXFunc(activeNode, _cardMover.GetPlaySpaceTopRight().x))
                .AppendInterval(_waitTime)
                .Append(_tweenYFunc(activeNode, finalHeight))
            );


        switch (cardType)
        {
            case CardType.DIALOGUE:
                inventoryNode[1].IsTopLevel = true;
                break;
        }

        int inventoryHeight = maxHeight;

        switch (cardType)
        {
            case CardType.DIALOGUE:
                inventoryHeight += inventoryNode[0].GetNodeCount(CardTraversal.CONTEXT) + 1;
                break;
            case CardType.KEY:
                inventoryHeight += inventoryNode.GetNodeCount(CardTraversal.CONTEXT);
                break;
        }

        entireSequence.Join(
            DOTween.Sequence()
                .Append(_tweenYFunc(inventoryNode, inventoryHeight))
                .AppendInterval(_waitTime)
                .AppendInterval(_horizontalTime)
                .AppendInterval(_waitTime)
                .Append(_tweenYFunc(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT) + 1))
            );

        // move all cards that were to the right of activeNode one space to the left

        for (int i = parentNode.Children.IndexOf(activeNode) + 1; i < parentNode.Children.Count; i++)
        {
            // ich setze nur die hier top level. Root ist hierbei nicht top-level was etwas weird ist
            _cardManager.AddToTopLevelMainPile(parentNode.Children[i]);
            
            entireSequence.Join(
            DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .AppendInterval(_waitTime)
                .Append(_tweenXFunc(parentNode.Children[i], _cardMover.GetChildrenDistance() * (i-1) - _cardMover.GetChildrenStartOffset()))
            );
        }

        return entireSequence;
    }
}
