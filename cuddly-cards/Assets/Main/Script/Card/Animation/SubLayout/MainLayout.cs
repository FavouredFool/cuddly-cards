using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class MainLayout : SubLayout
{
    public MainLayout(CardManager cardManager) : base(cardManager)
    {
    }

    public override void SetLayoutStatic(CardNode baseNode)
    {
        switch (_stateManager.States.Peek())
        {
            case CoverState:
                ResetCover(baseNode);
                break;
            case TalkState:
                ResetTalk(baseNode);
                break;
            case LockState:
            case InventoryState:
            case DialogueState:
                _subStatics.ResetBaseBackRoot(baseNode);
                break;
            default:
                ResetDefault(baseNode);
                break;
        }
    }

    public void ResetCover(CardNode baseNode)
    {
        _subStatics.SetNode(baseNode, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f, _cardMover.GetPlaySpaceBottomLeft().y));
    }

    public void ResetTalk(CardNode baseNode)
    {
        List<CardNode> childChildList = new();

        foreach (CardNode node in baseNode.Children)
        {
            _cardManager.AddToTopLevel(node);
            
            foreach (CardNode childchild in node.Children)
            {
                _cardManager.AddToTopLevel(childchild);
                childChildList.Add(childchild);
            }
        }

        int count = baseNode.Children.Count;
        for (int i = 0; i < count; i++)
        {
            _subStatics.FanOutCard(baseNode.Children[i], i, count, false);
        }

        _subStatics.ResetBaseBackRoot(baseNode);

        int height = 0;
        
        for (int i = childChildList.Count - 1; i >= 0; i--)
        {
            CardNode node = childChildList[i];

            // can not use node count below node because the sequence is broken -> gotta do it by hand.
            _cardMover.MoveCard(node, _cardMover.GetPlaySpaceBottomLeft());
            height += node.GetNodeCount(CardTraversal.CONTEXT);
            node.Body.SetHeight(height);
        }

        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());
        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));
        baseNode.Body.SetHeight(height + 1);
    }

    public void ResetDefault(CardNode baseNode)
    {
        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            _subStatics.SetChild(baseNode.Children[i], i);
        }

        _subStatics.ResetBaseBackRoot(baseNode);
    }
}
