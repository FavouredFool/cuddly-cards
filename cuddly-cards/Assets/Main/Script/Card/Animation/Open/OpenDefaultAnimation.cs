using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenDefaultAnimation : OpenParentAnimation
{
    public OpenDefaultAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween MoveChildren(CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        
        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            //_cardManager.AddToTopLevel(baseNode.Children[i]);

            sequence.Join(DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(_subAnimations.MoveNodeXToChild(baseNode.Children[i], baseNode.Children[i]))
                .Append(_subAnimations.MoveNodeYLowerPile(baseNode.Children[i])));
        }

        return sequence;
    }
}
