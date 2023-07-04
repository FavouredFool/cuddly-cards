
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class EnterInventoryPileAnimation : CardAnimation
{
    public EnterInventoryPileAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode previousActiveNode)
    {
        Sequence entireSequence = DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_subAnimations.MoveNodeToRight(_cardManager.CardInventory.InventoryNode));

        return entireSequence;
    }
}
