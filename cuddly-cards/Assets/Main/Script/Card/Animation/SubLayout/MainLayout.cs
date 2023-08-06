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
        // das schaut nur den obersten State an -> könnte Probleme mit Inventory oder CloseUp machen?

        switch (_stateManager.States.Peek())
        {
            case CoverState:
                ResetCover(baseNode);
                break;
            case LockState:
            case InventoryState:
                ResetClose(baseNode);
                break;
            case TalkState:
                ResetTalk(baseNode);
                break;
            case DialogueState:
                ResetDialogue(baseNode);
                break;
            default:
                ResetDefault(baseNode);
                break;
        }
    }

    public void ResetCover(CardNode baseNode)
    {
        _cardManager.AddToTopLevel(baseNode);
        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));

        _cardMover.MoveCard(baseNode, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f, _cardMover.GetPlaySpaceBottomLeft().y));
    }

    public void ResetDialogue(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;

        _cardManager.AddToTopLevel(baseNode);

        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevel(baseNode.Parent);
            baseNode.Parent.Body.SetHeight(baseNode.Parent.GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevel(rootNode);
                rootNode.Body.SetHeight(rootNode.GetNodeCount(CardTraversal.BODY));
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }

        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));
    }

    public void ResetTalk(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;

        List<CardNode> childChildList = new();

        _cardManager.AddToTopLevel(baseNode);

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

        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevel(baseNode.Parent);
            baseNode.Parent.Body.SetHeight(baseNode.Parent.GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevel(rootNode);
                rootNode.Body.SetHeight(rootNode.GetNodeCount(CardTraversal.BODY));
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }

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

    public void ResetClose(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        // move in deck -> move out inventory

        _cardManager.AddToTopLevel(baseNode);
        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));
        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevel(baseNode.Parent);
            baseNode.Parent.Body.SetHeight(baseNode.Parent.GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevel(rootNode);
                rootNode.Body.SetHeight(rootNode.GetNodeCount(CardTraversal.BODY));
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }
    }

    public void ResetDefault(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        _cardManager.AddToTopLevel(baseNode);
        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevel(baseNode.Children[i]);
            baseNode.Children[i].Body.SetHeight(baseNode.Children[i].GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Children[i], new Vector2(i * _cardMover.ChildrenDistance - _cardMover.ChildrenStartOffset, _cardMover.GetPlaySpaceBottomLeft().y));
        }

        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevel(baseNode.Parent);
            baseNode.Parent.Body.SetHeight(baseNode.Parent.GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevel(rootNode);
                rootNode.Body.SetHeight(rootNode.GetNodeCount(CardTraversal.BODY));
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }
    }
}
