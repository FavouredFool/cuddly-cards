
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static CardInfo;

public class ExitInventoryPileAnimation : InventoryAnimation
{
    public ExitInventoryPileAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween InventoryCardAnimation(CardNode inventoryNode)
    {
        return GetMoveOutOfFrameTween(inventoryNode);
    }

    public override Tween KeysAnimation(CardNode inventoryNode)
    {
        Sequence entireSequence = DOTween.Sequence();

        foreach (var childNode in inventoryNode.Children.Where(childNode => childNode.IsTopLevel))
        {
            entireSequence.Join(GetMoveOutOfFrameTween(childNode));
        }
  
        return entireSequence;
    }

    Tween GetMoveOutOfFrameTween(CardNode node)
    {
        return DOTween.Sequence()
                    .AppendInterval(_verticalTime + 2 * _horizontalTime + 2 * _waitTime)
                    .Append(_subAnimations.MoveNodeXToFarRight(node));
    }
}
