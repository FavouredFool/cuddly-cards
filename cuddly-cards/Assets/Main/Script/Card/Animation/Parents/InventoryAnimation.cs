
using DG.Tweening;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class InventoryAnimation : CardAnimation
{
    protected InventoryAnimation(CardManager cardManager) : base(cardManager) {}

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Join(InventoryCardAnimation(_cardManager.CardInventory.InventoryNode))
            .Join(KeysAnimation(_cardManager.CardInventory.InventoryNode));
    }

    public virtual Tween InventoryCardAnimation(CardNode inventoryNode)
    {
        return DOTween.Sequence();
    }

    public virtual Tween KeysAnimation(CardNode inventoryNode)
    {
        return DOTween.Sequence();
    }
}
