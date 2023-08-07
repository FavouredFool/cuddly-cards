
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class FromInventoryAnimation : InventoryAnimation
{
    bool _doDelay;
    public FromInventoryAnimation(CardManager cardManager, bool doDelay) : base(cardManager) {
        _doDelay = doDelay;
    }

    public override Tween InventoryCardAnimation(CardNode inventoryNode)
    {
        return _subAnimations.MoveNodeY(inventoryNode, inventoryNode.GetNodeCount(CardTraversal.CONTEXT));
    }

    public override Tween KeysAnimation(CardNode inventoryNode)
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevel(inventoryNode.Children[i]);
            sequence.Join(_subAnimations.FanInCard(inventoryNode.Children[i], inventoryNode, _doDelay, true));
        }

        return sequence;
    }
}
