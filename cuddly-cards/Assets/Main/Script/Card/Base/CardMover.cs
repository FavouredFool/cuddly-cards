using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static CardInfo;

public class CardMover : MonoBehaviour
{
    [Header("CardPositions")]
    [SerializeField] Vector2 _playSpaceBottomLeft = new(-2.5f, 0);

    [SerializeField] Vector2 _playSpaceTopRight = new(2.875f, 2.5f);

    [SerializeField, Range(0, 2f)] float _childrenDistance = 1.125f;

    [SerializeField] float _childrenStartOffset = 1;


    [Header("CardMovement")]
    [SerializeField] float _verticalTime = 0.5f;

    [SerializeField] float _horizontalTime = 1f;

    [SerializeField] float _waitTime = 1f;

    [Header("Inventory")]
    [SerializeField] float _border = 1f;

    [SerializeField] float _inventoryCardRotationAmount = 0.9f;

    [Header("Easing")]
    [SerializeField] Ease _horizontalEasing;

    [SerializeField] Ease _verticalEasing;

    public bool IsAnimatingFlag { get; set; } = false;

    public CardManager CardManager { get; set; }
    public SubAnimations SubAnimations { get; set; }

    public Ease HorizontalEasing => _horizontalEasing;
    public Ease VerticalEasing => _verticalEasing;

    public Vector2 PlaySpaceBottomLeft => _playSpaceBottomLeft;
    public Vector2 PlaySpaceTopRight => _playSpaceTopRight;

    public float InventoryCardRotationAmount => _inventoryCardRotationAmount;
    public float Border => _border;

    public float ChildrenStartOffset => _childrenStartOffset;
    public float ChildrenDistance => _childrenDistance;

    public float WaitTime => _waitTime;
    public float HorizontalTime => _horizontalTime;
    public float VerticalTime => _verticalTime;

    public void Start()
    {
        SubAnimations = new SubAnimations(CardManager);
    }

    public List<SubLayout> GetSubLayouts()
    {
        return new List<SubLayout>
        {
            new MainLayout(CardManager),
            new InventoryLayout(CardManager),
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
        foreach (CardNode topLevel in CardManager.GetTopLevelNodesMainPile())
        {
            topLevel.Children.Aggregate(1, (current, childNode) => current + childNode.SetPositionsRecursive(current));
        }
    }

    public void SetInventoryCardsRelativeToParent()
    {
        int size = 1;

        foreach (CardNode childNode in CardManager.CardInventory.InventoryNode.Children)
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
        CardNode inventoryNode = CardManager.CardInventory.InventoryNode;
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
        inventoryPart.Body.transform.localRotation = Quaternion.Euler(0, 0, -InventoryCardRotationAmount);

        for (int i = inventoryPart.Children.Count - 1; i >= 0; i--)
        {
            inventoryPart[i].Body.SetHeightFloat(2 + (i + 1) * -0.01f);
            inventoryPart[i].Body.transform.localRotation = Quaternion.Euler(0, 0, -InventoryCardRotationAmount);
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

    public void SetHeightOfTopLevelNodes()
    {
        foreach (CardNode node in CardManager.GetTopLevelNodesMainPile())
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

    public Tween TweenZ(CardNode main, float posZ)
    {
        return main.Body.transform.DOMoveZ(posZ, _horizontalTime).SetEase(_horizontalEasing);
    }
}