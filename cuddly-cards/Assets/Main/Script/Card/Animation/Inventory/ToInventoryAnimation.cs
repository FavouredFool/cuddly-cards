
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

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        // TODO Combine this with DisplayKeys because there are only keys left.

        Sequence entireSequence = DOTween.Sequence();

        float totalSpace = _cardMover.PlaySpaceTopRight.x - _cardMover.PlaySpaceBottomLeft.x;
        float fannedCardSpace = totalSpace - 2 * _cardMover.Border;

        entireSequence.Join(_subAnimations.MoveInventoryCardWhileFanning(1, _synchronizeLoweringWithFanning));

        float generalStartOffset = _cardMover.PlaySpaceBottomLeft.x + _cardMover.Border;

        entireSequence.Join(_subAnimations.FanOutCardsFromRight(generalStartOffset, fannedCardSpace));

        return entireSequence;
    }
}
