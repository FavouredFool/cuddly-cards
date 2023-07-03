
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ToInventoryAnimation : InventoryAnimation
{
    public ToInventoryAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        float totalSpace = _playSpaceTopRight.x - _playSpaceBottomLeft.x;
        float fannedCardSpace = (totalSpace - 3 * _cardMover.GetBorder()) * 0.5f;

        entireSequence.Join(_subAnimations.MoveInventoryCardWhileFanning(1));

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            float generalStartOffset = _playSpaceBottomLeft.x + (1 + (1 - i)) * _cardMover.GetBorder() + (1 - i) * fannedCardSpace;

            entireSequence.Join(_subAnimations.FanOutCardsFromRight(inventoryNode[i], generalStartOffset, fannedCardSpace));
        }

        return entireSequence;
    }
}
