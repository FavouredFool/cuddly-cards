
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class CollectCardAnimation : CardAnimation
{
    int _minRaiseHeight = 10;

    public CollectCardAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode parentNode = activeNode.Parent;

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;

        // activeNode vertical nach oben, relativ dem höchsten stack rechts der Node.

        int maxNeightbourHeight = 0;

        for(int i = parentNode.Children.IndexOf(activeNode) + 1; i < parentNode.Children.Count; i++)
        {
            int tempHeight = parentNode.Children[i].GetNodeCount(CardTraversal.CONTEXT);

            if (tempHeight > maxNeightbourHeight)
            {
                maxNeightbourHeight = tempHeight;
            }
        }

        maxNeightbourHeight += 1;
        maxNeightbourHeight = Math.Max(maxNeightbourHeight, _minRaiseHeight);

        entireSequence.Join(
            DOTween.Sequence()
                .Append(_subAnimations.MoveNodeY(activeNode, maxNeightbourHeight))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToRight(activeNode))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeY(activeNode, 1))
            );


        int inventoryHeight = maxNeightbourHeight + inventoryNode.GetNodeCount(CardTraversal.CONTEXT);

        entireSequence.Join(
            DOTween.Sequence()
                .Append(_subAnimations.MoveNodeY(inventoryNode, inventoryHeight))
                .AppendInterval(2 * _waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeY(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT) + 1))
            );

        // move all cards that were to the right of activeNode one space to the left

        for (int i = parentNode.Children.IndexOf(activeNode) + 1; i < parentNode.Children.Count; i++)
        {
            // ich setze nur die hier top level. Root ist hierbei nicht top-level was etwas weird ist
            _cardManager.AddToTopLevelMainPile(parentNode.Children[i]);

            entireSequence.Join(
            DOTween.Sequence()
                .AppendInterval(_verticalTime + _waitTime)
                .Append(_subAnimations.MoveNodeXChildOneSpaceToLeft(parentNode.Children[i]))
            );
        }

        return entireSequence;
    }
}
