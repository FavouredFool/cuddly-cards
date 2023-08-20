
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class BackDefaultToFanAnimation : BackParentAnimation
{
    public BackDefaultToFanAnimation(CardManager cardManager) : base(cardManager)
    {
    }


    public override Tween SetActiveNode(CardNode activeNode, CardNode baseNode)
    {
        int height = activeNode.GetNodeCount(CardTraversal.CONTEXT) - activeNode.Children.Count;

        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(activeNode, activeNode))
            .Append(_subAnimations.MoveNodeZNearer(activeNode))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(activeNode, height));
    }

    public override Tween AnimateChildrenAndBase(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(SetBaseNode(activeNode, baseNode));
        sequence.Join(SetBaseNodeChildren(activeNode, baseNode));
        sequence.Join(SetOtherChildren(activeNode, baseNode));
        sequence.Join(SetOtherChildrenChildren(activeNode, baseNode));

        return sequence;
    }

    public Tween SetBaseNode(CardNode activeNode, CardNode baseNode)
    {
        // BaseNode is being turned into a child.
        _cardManager.AddToTopLevel(baseNode);

        return DOTween.Sequence()
            .Append(_subAnimations.MoveNodeYLiftPile(baseNode, activeNode))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(_subAnimations.FanOutCard(baseNode, baseNode.Parent.Children.IndexOf(baseNode), baseNode.Parent.Children.Count, false));
    }

    public Tween SetBaseNodeChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // The children of the previous baseNode

        foreach (CardNode child in baseNode.Children)
        {
            // The height of all the children below them, but only the children. So you take cards below and subtract the amount of other children to the left of basenode
            int countUp = child.GetNodeCountUpToNodeInPile(activeNode, CardTraversal.CONTEXT) + 1;
            int subtract = baseNode.Parent.Children.Count - baseNode.Parent.Children.IndexOf(baseNode);

            int height = countUp - subtract;

            sequence.Join(DOTween.Sequence()
                .Append(_subAnimations.LiftAndMoveChildToBase(child, activeNode))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(_subAnimations.MoveNodeY(child, height))
                );
        }

        return sequence;
    }

    public Tween SetOtherChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < activeNode.Children.Count; i++)
        {
            CardNode child = activeNode.Children[i];

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
            .Append(_subAnimations.FanOutCard(child, i, activeNode.Children.Count, false))
            );
        }

        return sequence;
    }

    public Tween SetOtherChildrenChildren(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        // The children of the previous baseNode

        foreach (CardNode child in activeNode.Children)
        {
            if (child == baseNode)
            {
                continue;
            }

            foreach (CardNode childChild in child.Children)
            {
                // The height of all the children below them, but only the children. So you take cards below and subtract the amount of other children to the left of basenode
                int height = childChild.GetNodeCountUpToNodeInPile(activeNode, CardTraversal.CONTEXT) - child.Parent.Children.IndexOf(baseNode);

                sequence.Join(DOTween.Sequence()
                    .Append(_subAnimations.MoveNodeYLiftPile(childChild, activeNode))
                    .Append(_subAnimations.MoveNodeZNearer(activeNode))
                    .AppendInterval(_waitTime + _horizontalTime)
                    .Append(_subAnimations.MoveNodeY(childChild, height))
                    );
            }
        }

        return sequence;
    }



    //------
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
