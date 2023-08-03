using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class OpenParentAnimation : CardAnimation
{
    public OpenParentAnimation(CardManager cardManager) : base(cardManager) { }

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        // --- Variables
        CardNode rootNode = _cardManager.RootNode;

        // --- Sequence

        Sequence entireSequence = DOTween.Sequence();

        entireSequence.Join(DOTween.Sequence()
            .AppendInterval(_verticalTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeYLowerPile(baseNode)));

        entireSequence.Join(MoveChildren(baseNode));

        return entireSequence;
    }

    public abstract Tween MoveChildren(CardNode baseNode);
}
