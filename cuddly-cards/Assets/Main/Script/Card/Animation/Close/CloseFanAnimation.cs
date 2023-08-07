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
        Sequence sequence = DOTween.Sequence();

        List<CardNode> childChildList = new();

        int count = baseNode.Children.Count;

        for (int i = 0; i < count; i++)
        {
            CardNode node = baseNode.Children[i];

            foreach (CardNode childChild in node.Children)
            {
                _cardManager.AddToTopLevel(childChild, false);
                childChildList.Add(childChild);
            }

            sequence.Join(_subAnimations.FanInCard(node, baseNode, false, false));
        }

        for (int i = childChildList.Count - 1; i >= 0; i--)
        {
            CardNode node = childChildList[i];

            sequence.Join(_subAnimations.MoveNodeYLiftPile(node, baseNode));
        }

        return sequence;
    }

    public override Tween BaseAnimation(CardNode activeNode, CardNode baseNode)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(_subAnimations.MoveNodeYLiftPile(baseNode, baseNode));

        return sequence;
    }
}
