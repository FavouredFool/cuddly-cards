using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    [Header("CardPositions")]
    [SerializeField]
    Vector2 _playSpaceBottomLeft = new Vector2(-2.5f, 0);

    [SerializeField]
    Vector2 _playSpaceTopRight = new Vector2(2.875f, 2.5f);

    [SerializeField, Range(0, 2f)]
    float _childrenDistance = 1.125f;

    [SerializeField]
    float _childrenStartOffset = 1;


    [Header("CardMovement")]
    [SerializeField, Range(0f, 1)]
    float _verticalTime = 0.5f;

    [SerializeField, Range(0.1f, 210)]
    float _horizontalTime = 1f;

    [SerializeField, Range(0f, 210)]
    float _waitTime = 1f;

    [Header("Inventory")]
    [SerializeField]
    float _border = 1f;

    [Header("Easing")]
    [SerializeField]
    Ease _horizontalEasing;

    [SerializeField]
    Ease _verticalEasing;

    bool _isAnimating = false;

    public bool IsAnimatingFlag { get { return _isAnimating; } set { _isAnimating = value; } }

    CardManager _cardManager;
    CardInventory _cardInventory;
    StateManager _stateManager;

    List<CardAnimation> _cardAnimations;

    public void Awake()
    {
        _cardManager = GetComponent<CardManager>();
        _cardInventory = GetComponent<CardInventory>();
        _stateManager = GetComponent<StateManager>();
        _cardAnimations = new(){
            new ChildAnimation(_cardManager, this, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
            new BackAnimation(_cardManager, this, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
            new ToCoverAnimation(_cardManager, this, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
            new FromCoverAnimation(_cardManager, this, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
        };
    }

    public void LateUpdate()
    {
        // Move Cards with custom parenting structure per frame
        if (!IsAnimatingFlag)
        {
            return;
        }

        SetCardsRelativeToParent();
    }

    public void SetCardsRelativeToParent()
    {
        SetInventoryCardsRelativeToParent();
        SetMainCardsRelativeToParent();
    }

    public CardAnimation CardTransitionToAnimation(CardTransition transition)
    {
        switch (transition)
        {
            case CardTransition.CHILD:
                return _cardAnimations[0];
            case CardTransition.BACK:
                return _cardAnimations[1];
            case CardTransition.TOCOVER:
                return _cardAnimations[2];
            case CardTransition.FROMCOVER:
                return _cardAnimations[3];
        }

        Debug.Log("ANIMATION NOT SET YET");
        return null;
    }

    public async Task AnimateCards(CardNode activeNode, CardNode previousActiveNode, CardNode rootNode, CardTransition transition)
    {
        _isAnimating = true;
        await CardTransitionToAnimation(transition).AnimateCards(activeNode, previousActiveNode, rootNode);
        _isAnimating = false;
    }

    public void SetMainCardsRelativeToParent()
    {
        List<CardNode> topLevelNodes = _cardManager.GetTopLevelNodesMainPile();
        foreach (CardNode topLevel in topLevelNodes)
        {
            int size = 1;

            foreach (CardNode childNode in topLevel.Children)
            {
                size += childNode.SetPositionsRecursive(size);
            }
        }
    }

    public void SetInventoryCardsRelativeToParent()
    {
        int size = 1;

        foreach (CardNode childNode in _cardInventory.GetInventoryNode().Children)
        {
            if (childNode.IsTopLevel)
            {
                return;
            }

            size += childNode.SetPositionsRecursive(size);
        }
    }

    public void ResetPosition(CardNode rootNode)
    {
        rootNode.TraverseChildren(CardInfo.CardTraversal.CONTEXT, delegate (CardNode node)
        {
            node.Body.transform.localPosition = Vector3.zero;
            return true;
        });
    }

    public void MoveCard(CardNode card, Vector2 position)
    {
        card.Body.transform.localPosition = new Vector3(position.x, card.Body.transform.localPosition.y, position.y);
    }

    public void SetHeights()
    {
        SetHeightOfTopLevelNodes();
        SetHeightOfInventory();
    }

    public void SetHeightOfTopLevelNodes()
    {
        foreach (CardNode node in _cardManager.GetTopLevelNodesMainPile())
        {
            node.Body.SetHeight(node.GetNodeCount(CardInfo.CardTraversal.BODY));
        }
    }

    public void SetHeightOfInventory()
    {
        CardNode inventoryNode = _cardInventory.GetInventoryNode();
        inventoryNode.Body.SetHeight(inventoryNode.GetNodeCount(CardTraversal.BODY));

        int cardNr = inventoryNode[0].Children.Count+1;
        inventoryNode[0].Body.SetHeight(cardNr);

        for (int i = inventoryNode[0].Children.Count-1; i >= 0; i--)
        {
            cardNr -= 1;
            inventoryNode[0][i].Body.SetHeight(cardNr);
        }

        cardNr = inventoryNode[1].Children.Count + 1;
        inventoryNode[1].Body.SetHeight(cardNr);

        for (int i = inventoryNode[1].Children.Count - 1; i >= 0; i--)
        {
            cardNr -= 1;
            inventoryNode[1][i].Body.SetHeight(cardNr);
        }
    }

    public void MoveCardsForLayoutStatic(CardNode activeNode, CardNode rootNode, bool isStartLayout)
    {
        if (_cardInventory.InventoryIsOpenFlag != _cardInventory.InventoryShouldOpenFlag)
        {
            if (_cardInventory.InventoryShouldOpenFlag)
            {
                // close cards because inventory opens
                CloseCards(activeNode, rootNode);
            }
            else
            {
                // TODO THIS IS TERRIBLE AAAA
                if (isStartLayout)
                {
                    MoveCardsForStartLayoutStatic(rootNode);
                }
                else
                {
                    MoveCardsForGeneralLayoutStatic(activeNode, rootNode);
                }
            }
        }
        else
        {
            if (isStartLayout)
            {
                MoveCardsForStartLayoutStatic(rootNode);
            }
            else
            {
                MoveCardsForGeneralLayoutStatic(activeNode, rootNode);
            }
        }

        
        // this always does its thing and espects the other cards to behave properly
        MoveCardsForInventoryStatic(_cardInventory.GetInventoryNode(), _cardInventory.InventoryShouldOpenFlag);
    }

    public void MoveCardsForStartLayoutStatic(CardNode rootNode)
    {
        _cardManager.AddToTopLevelMainPile(rootNode);
        MoveCard(rootNode, new Vector2(_playSpaceBottomLeft.x  + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f, _playSpaceBottomLeft.y));
    }

    public void MoveCardsForGeneralLayoutStatic(CardNode pressedNode, CardNode rootNode)
    {
        _cardManager.AddToTopLevelMainPile(pressedNode);
        MoveCard(pressedNode, _playSpaceBottomLeft);

        if (pressedNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(pressedNode.Parent);
            MoveCard(pressedNode.Parent, new Vector2(_playSpaceBottomLeft.x, _playSpaceTopRight.y));

            if (pressedNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                MoveCard(rootNode, _playSpaceTopRight);
            }
        }

        for (int i = 0; i < pressedNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(pressedNode.Children[i]);
            MoveCard(pressedNode.Children[i], new Vector2(i * _childrenDistance - _childrenStartOffset, _playSpaceBottomLeft.y));
        }
    }

    public void CloseCards(CardNode pressedNode, CardNode rootNode)
    {
        _cardManager.AddToTopLevelMainPile(pressedNode);
        MoveCard(pressedNode, _playSpaceBottomLeft);

        if (pressedNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(pressedNode.Parent);
            MoveCard(pressedNode.Parent, new Vector2(_playSpaceBottomLeft.x, _playSpaceTopRight.y));

            if (pressedNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                MoveCard(rootNode, _playSpaceTopRight);
            }
        }
    }

    public void MoveCardsForInventoryStatic(CardNode inventoryNode, bool inventoryIsOpen)
    {
        if (inventoryIsOpen)
        {
            // Set all cardnodes toplevel
            inventoryNode[0].IsTopLevel = true;
            foreach (CardNode node in inventoryNode[0].Children)
            {
                node.IsTopLevel = true;
            }
            inventoryNode[1].IsTopLevel = true;
            foreach (CardNode node in inventoryNode[1].Children)
            {
                node.IsTopLevel = true;
            }

            float totalSpace = _playSpaceTopRight.x - _playSpaceBottomLeft.x;
            float fannedCardSpace = (totalSpace - 3 * _border) * 0.5f;

            float dialogueOffset = _playSpaceBottomLeft.x + 2 * _border + fannedCardSpace;
            FanCardsFromInventorySubcard(inventoryNode[0], dialogueOffset, fannedCardSpace);

            float keyOffset = _playSpaceBottomLeft.x + _border;
            FanCardsFromInventorySubcard(inventoryNode[1], keyOffset, fannedCardSpace);
        }
        else
        {
            // Set no cardnodes toplevel
            inventoryNode[0].IsTopLevel = false;
            foreach (CardNode node in inventoryNode[0].Children)
            {
                node.IsTopLevel = false;
            }
            inventoryNode[1].IsTopLevel = false;
            foreach (CardNode node in inventoryNode[1].Children)
            {
                node.IsTopLevel = false;
            }
        }

        MoveCard(inventoryNode, new Vector2(_playSpaceTopRight.x, _playSpaceBottomLeft.y));
    }

    public void FanCardsFromInventorySubcard(CardNode inventorySubcard, float startFanX, float fannedCardSpace)
    {
        int totalChildCards = inventorySubcard.Children.Count;

        MoveCard(inventorySubcard, new Vector2(startFanX + fannedCardSpace, _playSpaceBottomLeft.y));

        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildCards);

        for (int i = 0; i < totalChildCards; i++)
        {
            MoveCard(inventorySubcard[i], new Vector2(startFanX + i * CardInfo.CARDWIDTH * cardPercentage, _playSpaceBottomLeft.y));
        }
    }

    public void MoveCardsForToggleInventoryAnimated()
    {
        IsAnimatingFlag = true;
        float timeTotal = _verticalTime * 2 + _horizontalTime * 3 + 2 * _waitTime;

        // MAKE SURE THAT THE TIMER IS A BIT LONGER THAN THE MAXIMUM TWEENING TIME
        Sequence timerSequence = DOTween.Sequence()
            .AppendInterval(timeTotal + 0.01f)
            .OnComplete(() => { IsAnimatingFlag = false; _cardManager.FinishLayout(false); });
    }

    public Vector2 GetPlaySpaceBottomLeft()
    {
        return _playSpaceBottomLeft;
    }
    
    public Vector2 GetPlaySpaceTopRight()
    {
        return _playSpaceTopRight;
    }

    public Tween TweenX(CardNode main, float posX)
    {
        return main.Body.transform.DOMoveX(posX, _horizontalTime).SetEase(_horizontalEasing);
    }

    public Tween TweenY(CardNode main, int height)
    {
        return main.Body.transform.DOMoveY(height * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing);
    }

    public Tween TweenZ(CardNode main, float posZ)
    {
        return main.Body.transform.DOMoveZ(posZ, _horizontalTime).SetEase(_horizontalEasing);
    }

    public float GetChildrenDistance()
    {
        return _childrenDistance;
    }

    public float GetChildrenStartOffset()
    {
        return _childrenStartOffset;
    }

    public float GetWaitTime()
    {
        return _waitTime;
    }

    public float GetVerticalTime()
    {
        return _verticalTime;
    }

    public float GetHorizontalTime()
    {
        return _horizontalTime;
    }

    public Ease GetVerticalEase()
    {
        return _verticalEasing;
    }

    public Ease GetHorizontalEase()
    {
        return _horizontalEasing;
    }
}