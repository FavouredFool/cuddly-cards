
using DG.Tweening;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class MainAnimation : CardAnimation
{
    protected MainAnimation(CardManager cardManager) : base(cardManager) {}

    public override Sequence GetAnimationSequence(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence()
            .Join(OtherAnimation(activeNode, baseNode))
            .Join(ChildAnimation(activeNode, baseNode))
            .Join(BaseAnimation(activeNode, baseNode))
            .Join(BackAnimation(activeNode, baseNode))
            .Join(RootAnimation(activeNode, baseNode));
    }

    public virtual Tween ChildAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }

    public virtual Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }

    public virtual Tween BackAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }

    public virtual Tween RootAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }

    public virtual Tween OtherAnimation(CardNode activeNode, CardNode baseNode)
    {
        return DOTween.Sequence();
    }
}
