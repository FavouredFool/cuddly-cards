
using DG.Tweening;
using System.Threading.Tasks;
using System;
using UnityEngine;
using static CardInfo;

public abstract class InventoryAnimation : CardAnimation
{
    protected InventoryAnimation(CardManager cardManager) : base(cardManager) { }

}
