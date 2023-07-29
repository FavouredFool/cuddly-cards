
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class DisplayKeysAnimation : InventoryAnimation
{
    public DisplayKeysAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        CardNode keyParentNode = _cardManager.CardInventory.KeyParentNode;

        float totalSpace = _cardMover.PlaySpaceTopRight.x - _cardMover.PlaySpaceBottomLeft.x;
        float fannedCardSpace = totalSpace - 2 * _cardMover.Border;

        entireSequence.Join(_subAnimations.MoveInventoryCardWhileFanning(1, false));

        float generalStartOffset = _cardMover.PlaySpaceBottomLeft.x + _cardMover.Border;

        entireSequence.Join(_subAnimations.FanOutCardsFromRight(keyParentNode, generalStartOffset, fannedCardSpace));

        return entireSequence;
    }
}
