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
                ResetTalkHeight(baseNode);
                break;
            default:
                ResetDefault(baseNode);
                break;
        }
    }

    public void ResetCover(CardNode baseNode)
    {
        _cardManager.AddToTopLevelMainPile(baseNode);
        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));

        _cardMover.MoveCard(baseNode, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f, _cardMover.GetPlaySpaceBottomLeft().y));
    }

    public void ResetTalkHeight(CardNode baseNode)
    {
        // get the highest childamount -> take that as the baseheight (this wont work well for extreme cases)
        
        int maxHeight = 2;

        foreach (CardNode node in baseNode.Children)
        {
            int thisHeight = node.GetNodeCount(CardInfo.CardTraversal.CONTEXT);

            if (thisHeight > maxHeight)
            {
                maxHeight = thisHeight;
            }
        }

        for (int i = baseNode.Children.Count - 1; i >= 0; i--)
        {
            baseNode[i].Body.SetHeightFloat(maxHeight + (i * -0.01f));
            baseNode[i].Body.transform.localRotation = Quaternion.Euler(0, 0, _cardMover.InventoryCardRotationAmount);
        }
    }

    public void ResetTalk(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;

        _cardManager.AddToTopLevelMainPile(baseNode);

        foreach (CardNode node in baseNode.Children)
        {
            _cardManager.AddToTopLevelMainPile(node);
        }

        float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
        float fannedCardSpace = totalSpace - 2 * _cardMover.Border;

        float offset = _cardMover.GetPlaySpaceTopRight().x - _cardMover.Border;
        FanCardsFromTalkStatic(baseNode, offset, fannedCardSpace);

        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);
            baseNode.Parent.Body.SetHeight(baseNode.Parent.GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                rootNode.Body.SetHeight(rootNode.GetNodeCount(CardTraversal.BODY));
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }

        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));
    }

    void FanCardsFromTalkStatic(CardNode talkNode, float startFanX, float fannedCardSpace)
    {
        int totalChildCards = talkNode.Children.Count;

        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildCards - 1);

        for (int i = 0; i < totalChildCards; i++)
        {
            _cardMover.MoveCard(talkNode.Children[totalChildCards - i - 1], new Vector2(startFanX - i * CardInfo.CARDWIDTH * cardPercentage, _cardMover.GetPlaySpaceBottomLeft().y));
        }
    }

    public void ResetClose(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        // move in deck -> move out inventory

        _cardManager.AddToTopLevelMainPile(baseNode);
        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));
        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);
            baseNode.Parent.Body.SetHeight(baseNode.Parent.GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                rootNode.Body.SetHeight(rootNode.GetNodeCount(CardTraversal.BODY));
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }
    }

    public void ResetDefault(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        _cardManager.AddToTopLevelMainPile(baseNode);
        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Children[i]);
            baseNode.Children[i].Body.SetHeight(baseNode.Children[i].GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Children[i], new Vector2(i * _cardMover.ChildrenDistance - _cardMover.ChildrenStartOffset, _cardMover.GetPlaySpaceBottomLeft().y));
        }

        baseNode.Body.SetHeight(baseNode.GetNodeCount(CardTraversal.BODY));

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);
            baseNode.Parent.Body.SetHeight(baseNode.Parent.GetNodeCount(CardTraversal.BODY));
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                rootNode.Body.SetHeight(rootNode.GetNodeCount(CardTraversal.BODY));
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }
    }
}
