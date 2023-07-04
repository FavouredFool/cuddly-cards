
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class DisplayKeysAnimation : InventoryAnimation
{
    public DisplayKeysAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode inventoryNode = _cardInventory.GetInventoryNode();

        float totalSpace = _playSpaceTopRight.x - _playSpaceBottomLeft.x;
        float fannedCardSpace = totalSpace - 2 * _cardMover.GetBorder();

        entireSequence.Join(_subAnimations.MoveInventoryCardWhileFanning(inventoryNode[0].GetNodeCount(CardTraversal.CONTEXT) + 1, false));

        float generalStartOffset = _playSpaceBottomLeft.x + _cardMover.GetBorder();

        entireSequence.Join(_subAnimations.FanOutCardsFromRight(inventoryNode[1], generalStartOffset, fannedCardSpace));

        return entireSequence;
    }
}
