
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ToInventoryAnimation : CardAnimation
{
    bool _synchronizeLoweringWithFanning;
    
    public ToInventoryAnimation(CardManager cardManager, bool synchronizeLoweringWithFanning) : base(cardManager)
    {
        _synchronizeLoweringWithFanning = synchronizeLoweringWithFanning;
    }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;

        entireSequence.Join(_subAnimations.MoveInventoryCardWhileFanning(1, _synchronizeLoweringWithFanning));

        int count = inventoryNode.Children.Count;

        for (int i = 0; i < count; i++)
        {
            CardNode node = inventoryNode.Children[i];
            _cardManager.AddToTopLevel(node);
            
            entireSequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.FanOutCards(node, i, count, true)));
        }

        return entireSequence;
    }
}
