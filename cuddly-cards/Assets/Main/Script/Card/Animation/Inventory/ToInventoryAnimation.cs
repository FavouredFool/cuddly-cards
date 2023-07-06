
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class ToInventoryAnimation : InventoryAnimation
{
    public ToInventoryAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;

        float totalSpace = _cardMover.PlaySpaceTopRight.x - _cardMover.PlaySpaceBottomLeft.x;
        float fannedCardSpace = (totalSpace - 3 * _cardMover.Border) * 0.5f;

        entireSequence.Join(_subAnimations.MoveInventoryCardWhileFanning(1, true));

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            float generalStartOffset = _cardMover.PlaySpaceBottomLeft.x + (1 + (1 - i)) * _cardMover.Border + (1 - i) * fannedCardSpace;

            entireSequence.Join(_subAnimations.FanOutCardsFromRight(inventoryNode[i], generalStartOffset, fannedCardSpace));
        }

        return entireSequence;
    }
}
