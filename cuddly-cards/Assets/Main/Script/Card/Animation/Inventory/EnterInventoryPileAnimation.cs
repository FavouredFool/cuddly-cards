
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class EnterInventoryPileAnimation : InventoryAnimation
{
    public EnterInventoryPileAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween InventoryCardAnimation(CardNode inventoryNode)
    {
        return DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(_subAnimations.MoveNodeXToRight(_cardManager.CardInventory.InventoryNode));
    }
}
