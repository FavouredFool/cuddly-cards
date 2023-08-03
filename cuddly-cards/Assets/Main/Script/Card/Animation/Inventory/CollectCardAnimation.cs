
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
        // --- Variables

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;

        // --- Sequence

        Sequence entireSequence = DOTween.Sequence();

        int maxNeightbourHeight = 0;

        for(int i = baseNode.Children.IndexOf(activeNode) + 1; i < baseNode.Children.Count; i++)
        {
            int tempHeight = baseNode.Children[i].GetNodeCount(CardTraversal.CONTEXT);

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

        for (int i = baseNode.Children.IndexOf(activeNode) + 1; i < baseNode.Children.Count; i++)
        {
            entireSequence.Join(
            DOTween.Sequence()
                .AppendInterval(_verticalTime + _waitTime)
                .Append(_subAnimations.MoveNodeXChildOneSpaceToLeft(baseNode.Children[i]))
            );
        }

        return entireSequence;
    }
}
