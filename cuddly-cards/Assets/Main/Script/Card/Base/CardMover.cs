using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CardMover : MonoBehaviour
{
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
    [SerializeField]
    float _verticalTime = 0.5f;

    [SerializeField]
    float _horizontalTime = 1f;

    [SerializeField]
    float _waitTime = 1f;

    [Header("Inventory")]
    [SerializeField]
    float _border = 1f;
    [SerializeField]
    float _inventoryCardRotationAmount = 0.9f;
    [SerializeField]
    float _onHoverMovement = 0.5f;

    [Header("Easing")]
    [SerializeField]
    Ease _horizontalEasing;

    [SerializeField]
    Ease _verticalEasing;

    bool _isAnimating = false;
    public bool IsAnimatingFlag { get { return _isAnimating; } set { _isAnimating = value; } }

    CardManager _cardManager;
    public CardManager CardManager { get { return _cardManager; } set { _cardManager = value; } }

    SubAnimations _subAnimations;
    public SubAnimations SubAnimations { get { return _subAnimations; } set { _subAnimations = value; } }

    public void Start()
    {
        _subAnimations = new SubAnimations(_cardManager);
    }

    public List<SubLayout> GetSubLayouts()
    {
        return new()
        {
            new MainLayout(_cardManager),
            new InventoryLayout(_cardManager),
        };
    }

    public void LateUpdate()
    {
        // Move Cards with custom parenting structure per frame
        if (!IsAnimatingFlag)
        {
            return;
        }
        
        SetMainCardsRelativeToParent();
        SetInventoryCardsRelativeToParent();
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

        foreach (CardNode childNode in _cardManager.CardInventory.GetInventoryNode().Children)
        {
            if (childNode.IsTopLevel)
            {
                return;
            }

            size += childNode.SetPositionsRecursive(size);
        }
    }


    public void SetHeightAndRotationOfInventory()
    {
        CardNode inventoryNode = _cardManager.CardInventory.GetInventoryNode();
        inventoryNode.Body.SetHeight(inventoryNode.GetNodeCount(CardTraversal.BODY));

        for (int i = 0; i < inventoryNode.Children.Count; i++)
        {
            if (inventoryNode[i].IsTopLevel)
            {
                SetFannedHeightAndRotationOfInventoryPart(inventoryNode[i]);
            }
            else
            {
                SetStackedHeightAndRotationOfInventoryPart(inventoryNode[i]);
            }
        }
    }

    public void SetStackedHeightAndRotationOfInventoryPart(CardNode inventoryPart)
    {
        int cardNr = inventoryPart.Children.Count + 1;
        inventoryPart.Body.SetHeight(cardNr);
        inventoryPart.Body.transform.localRotation = Quaternion.identity;

        for (int i = inventoryPart.Children.Count - 1; i >= 0; i--)
        {
            cardNr -= 1;
            inventoryPart[i].Body.SetHeight(cardNr);
            inventoryPart[i].Body.transform.localRotation = Quaternion.identity;
        }
    }

    public void SetFannedHeightAndRotationOfInventoryPart(CardNode inventoryPart)
    {
        inventoryPart.Body.SetHeight(2);
        inventoryPart.Body.transform.localRotation = Quaternion.Euler(0, 0, -GetInventoryCardRotationAmount());

        for (int i = inventoryPart.Children.Count - 1; i >= 0; i--)
        {
            inventoryPart[i].Body.SetHeight(2 + (i + 1) * -0.01f);
            inventoryPart[i].Body.transform.localRotation = Quaternion.Euler(0, 0, -GetInventoryCardRotationAmount());
        }
    }


    public void SetInventoryPosition()
    {
        float xInventoryPosition = _playSpaceTopRight.x;
        MoveCard(_cardManager.CardInventory.GetInventoryNode(), new Vector2(xInventoryPosition, _playSpaceBottomLeft.y));
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

    public void SetHeightOfTopLevelNodes()
    {
        foreach (CardNode node in _cardManager.GetTopLevelNodesMainPile())
        {
            node.Body.SetHeight(node.GetNodeCount(CardInfo.CardTraversal.BODY));
        }
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

    public Tween TweenY(CardNode main, float height)
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

    public float GetBorder()
    {
        return _border;
    }

    public float GetInventoryCardRotationAmount()
    {
        return _inventoryCardRotationAmount;
    }
}