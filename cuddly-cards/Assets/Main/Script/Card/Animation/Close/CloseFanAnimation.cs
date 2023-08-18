using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CloseFanAnimation : MainAnimation
{
    public CloseFanAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
        //return _subAnimations.FanIn(baseNode, activeNode);
    }

    public override Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }
}
