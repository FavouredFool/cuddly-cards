using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            default:
                ResetDefault(baseNode);
                break;
        }
    }

    public void ResetCover(CardNode baseNode)
    {
        _cardManager.AddToTopLevelMainPile(baseNode);
        _cardMover.MoveCard(baseNode, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f, _cardMover.GetPlaySpaceBottomLeft().y));
    }

    public void ResetClose(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        // move in deck -> move out inventory

        _cardManager.AddToTopLevelMainPile(baseNode);
        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }
    }

    public void ResetDefault(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        _cardManager.AddToTopLevelMainPile(baseNode);
        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }

        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Children[i]);
            _cardMover.MoveCard(baseNode.Children[i], new Vector2(i * _cardMover.ChildrenDistance - _cardMover.ChildrenStartOffset, _cardMover.GetPlaySpaceBottomLeft().y));
        }
    }
}
