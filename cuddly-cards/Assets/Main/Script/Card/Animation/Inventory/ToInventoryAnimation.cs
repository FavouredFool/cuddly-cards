
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ToInventoryAnimation : InventoryAnimation
{
    bool _synchronizeLoweringWithFanning;
    
    public ToInventoryAnimation(CardManager cardManager, bool synchronizeLoweringWithFanning) : base(cardManager)
    {
        _synchronizeLoweringWithFanning = synchronizeLoweringWithFanning;
    }

    public override Tween InventoryCardAnimation(CardNode inventoryNode)
    {
        return _subAnimations.MoveInventoryCardWhileFanning(1, _synchronizeLoweringWithFanning);
    }

    public override Tween KeysAnimation(CardNode inventoryNode)
    {
        Sequence sequence = DOTween.Sequence();

        int count = inventoryNode.Children.Count;

        for (int i = 0; i < count; i++)
        {
            CardNode node = inventoryNode.Children[i];
            _cardManager.AddToTopLevel(node);

            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.FanOutCard(node, i, count, true)));
        }

        return sequence;
    }
}
