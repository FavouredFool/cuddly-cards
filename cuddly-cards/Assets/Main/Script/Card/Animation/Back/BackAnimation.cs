
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackAnimation : BackParentAnimation
{
    bool _fromFan;
    bool _toFan;

    public BackAnimation(CardManager cardManager, bool fromFan, bool toFan) : base(cardManager)
    {
        _fromFan = fromFan;
        _toFan = toFan;
    }

    public override Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(SetOtherChildren(activeNode, baseNode));
        sequence.Join(SetBaseChild(activeNode, baseNode));

        return sequence;
    }

    public Tween SetBaseChild(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(FromDone(activeNode, baseNode));


        _cardManager.AddToTopLevel(baseNode);
        int index = activeNode.Children.IndexOf(baseNode);
        int count = activeNode.Children.Count;

        sequence.Join(DOTween.Sequence()
            .Append(BaseChildTweenIn(baseNode, activeNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(BaseChildTweenOut(baseNode, index, count)));

        
        foreach (CardNode childchild in baseNode.Children)
        {
            _cardManager.AddToTopLevel(childchild);

            sequence.Join(DOTween.Sequence()
                .Append(ChildTweenIn(childchild, activeNode))
                .AppendInterval(_waitTime)
                .Append(ChildChildTweenOut()));
        }
        

        return sequence;
    }

    public Tween FromDone(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        if (_fromFan)
        {
            sequence.Append(_subAnimations.FanIn(baseNode, activeNode));
        }
        else
        {
            Sequence fromSubSequence = DOTween.Sequence();

            fromSubSequence.Join(_subAnimations.MoveNodeYLiftPile(baseNode, activeNode));

            foreach (CardNode child in baseNode.Children)
            {
                _cardManager.AddToTopLevel(child);

                fromSubSequence.Join(ChildTweenIn(child, activeNode));
            }

            sequence.Append(fromSubSequence);
        }

        return sequence;
    }


    public Tween BaseChildTweenIn(CardNode baseNode, CardNode activeNode)
    {
        return _subAnimations.MoveNodeYLiftPile(baseNode, activeNode);
    }

    public Tween BaseChildTweenOut(CardNode baseNode, int index, int count)
    {
        if (_toFan)
        {
            return _subAnimations.FanOutCard(baseNode, index, count, false);
        }
        else
        {
            return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeXToChild(baseNode, baseNode))
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode));
        }
    }

    public Tween ChildTweenIn(CardNode childchild, CardNode activeNode)
    {
        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(childchild, activeNode))
            .Append(_subAnimations.MoveNodeXToLeft(childchild));
    }

    public Tween ChildChildTweenOut()
    {
        if (_toFan)
        {
            return DOTween.Sequence();
        }
        else
        {
            return DOTween.Sequence();
        }
    }


    // Other children

    public Tween SetOtherChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        Tween tweenForEnd;

        int count = activeNode.Children.Count;
        for (int i = 0; i < count; i++)
        {
            CardNode child = activeNode.Children[i];

            if (child == baseNode)
            {
                break;
            }

            _cardManager.AddToTopLevel(child);

            sequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(child, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(child))
            .AppendInterval(_waitTime)
            .Append(OtherChildrenTweenOut(child, i, count)));
        }

        return sequence;
    }

    public Tween OtherChildrenTweenOut(CardNode node, int index, int count)
    {
        if (_toFan)
        {
            return _subAnimations.FanOutCard(node, index, count, false);
        }
        else
        {
            return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeXToChild(node, node))
            .Append(_subAnimations.MoveNodeYLiftPile(node, node));
        }
    }

}
