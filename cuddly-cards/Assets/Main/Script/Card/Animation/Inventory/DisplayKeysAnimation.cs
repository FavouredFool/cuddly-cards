
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

        CardNode inventoryNode = _cardManager.CardInventory.InventoryNode;

        float totalSpace = _cardMover.PlaySpaceTopRight.x - _cardMover.PlaySpaceBottomLeft.x;
        float fannedCardSpace = totalSpace - 2 * _cardMover.Border;

        entireSequence.Join(_subAnimations.MoveInventoryCardWhileFanning(inventoryNode[0].GetNodeCount(CardTraversal.CONTEXT) + 1, false));

        float generalStartOffset = _cardMover.PlaySpaceBottomLeft.x + _cardMover.Border;

        entireSequence.Join(_subAnimations.FanOutCardsFromRight(inventoryNode[1], generalStartOffset, fannedCardSpace));

        return entireSequence;
    }
}
