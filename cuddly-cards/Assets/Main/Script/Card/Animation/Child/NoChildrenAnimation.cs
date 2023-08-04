
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class NoChildrenAnimation : ChildParentAnimation
{
    public NoChildrenAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween AnimateChildren(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }

}
