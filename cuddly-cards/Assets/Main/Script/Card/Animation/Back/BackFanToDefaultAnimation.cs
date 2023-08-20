
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackFanToDefaultAnimation : BackParentAnimation
{
    public BackFanToDefaultAnimation(CardManager cardManager) : base(cardManager)
    {
    }

    public override Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(SetBaseNode(activeNode, baseNode));
        sequence.Join(SetBaseNodeChildren(activeNode, baseNode));
        sequence.Join(SetBaseNodeChildrenChildren(activeNode, baseNode));
        sequence.Join(SetOtherChildren(activeNode, baseNode));

        return sequence;
    }

    public Tween SetBaseNode(CardNode activeNode, CardNode baseNode)
    {
        // BaseNode is being turned into a child.
        _cardManager.AddToTopLevel(baseNode);

        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, activeNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_subAnimations.MoveNodeXToChild(baseNode, baseNode))
            .Append(_subAnimations.MoveNodeY(baseNode, baseNode.GetNodeCount(CardTraversal.CONTEXT)));
    }

    public Tween SetBaseNodeChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // The children of the previous baseNode

        foreach (CardNode child in baseNode.Children)
        {
            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.FanInCard(child, activeNode, false, false))
                .AppendInterval(_waitTime)
                .Append(_subAnimations.MoveNodeXToChild(child, baseNode))
                .Append(_subAnimations.MoveNodeY(child, child.GetNodeCountUpToNodeInPile(baseNode, CardTraversal.CONTEXT)))
                );
        }

        return sequence;
    }

    public Tween SetBaseNodeChildrenChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // the children of the children that are initially below the baseNode

        foreach (CardNode child in baseNode.Children)
        {
            foreach (CardNode childChild in child.Children)
            {
                _cardManager.AddToTopLevel(childChild);

                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(childChild, activeNode))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(_subAnimations.MoveNodeXToChild(childChild, baseNode))
                    .Append(_subAnimations.MoveNodeY(childChild, childChild.GetNodeCountUpToNodeInPile(baseNode, CardTraversal.CONTEXT)))
                    );
            }

            
        }

        return sequence;
    }

    public Tween SetOtherChildren(CardNode activeNode, CardNode baseNode)
    {

        // Same as DefaultToDefault -> Can be combined
        Sequence sequence = DOTween.Sequence();

        foreach (CardNode child in activeNode.Children)
        {
            if (child == baseNode)
            {
                continue;
            }

            // Other children
            _cardManager.AddToTopLevel(child);

            sequence.Join(DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(child, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(child))
            .AppendInterval(_waitTime)
            .Append(_subAnimations.MoveOutChildFromBase(child))
            );
        }

        return sequence;
    }

    // ---------------------

    /*
    public override Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        //sequence.Join(SetOtherChildren(activeNode, baseNode));
        //sequence.Join(SetBaseChild(activeNode, baseNode));

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
                .Append(_subAnimations.MoveNodeYLowerPile(baseNode));
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
    */

}
