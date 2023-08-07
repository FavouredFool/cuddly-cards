using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class OpenParentAnimation : MainAnimation
{
    public OpenParentAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        return MoveChildren(baseNode);
    }

    public override Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        return MoveBaseNode(baseNode);
    }

    public abstract Tween MoveChildren(CardNode baseNode);

    public virtual Tween MoveBaseNode(CardNode baseNode)
    {
        return DOTween.Sequence()
            .AppendInterval(_verticalTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeYLowerPile(baseNode));
    }
}
