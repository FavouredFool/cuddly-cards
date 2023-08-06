using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class OpenFanAnimation : OpenParentAnimation
{

    int _height = 0;

    public OpenFanAnimation(CardManager cardManager) : base(cardManager) { }

    public override Tween MoveChildren(CardNode baseNode)
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

            sequence.Join(
                DOTween.Sequence()
                    .AppendInterval(_verticalTime)
                    .Append(_subAnimations.FanOutCard(node, i, count, false))
            );
        }

        for (int i = childChildList.Count - 1; i >= 0; i--)
        {
            CardNode node = childChildList[i];

            _height += node.GetNodeCount(CardTraversal.CONTEXT);

            sequence.Join(
                DOTween.Sequence()
                    .AppendInterval(_verticalTime + _horizontalTime)
                    .Append(_subAnimations.MoveNodeY(node, _height)));
        }

        return sequence;
    }

    public override Tween MoveBaseNode(CardNode baseNode)
    {
        return DOTween.Sequence()
            .AppendInterval(_verticalTime + _horizontalTime)
            .Append(_subAnimations.MoveNodeY(baseNode, _height + 1));
    }
}
