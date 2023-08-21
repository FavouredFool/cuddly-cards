using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeAnimationParent : MainAnimation
{
    protected FreeAnimationParent(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        _cardManager.ClearTopLevelNodesMainPile();

        return base.GetAnimationSequence(activeNode, baseNode);
    }
}
