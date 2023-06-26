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

    List<SubLayout> _subLayouts;
    

    public void Awake()
    {
        _cardManager = GetComponent<CardManager>();
        _cardInventory = GetComponent<CardInventory>();

        List<CardAnimation> mainAnimations = new()
        {
            new ChildAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
            new BackAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
            new ToCoverAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
            new FromCoverAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
            new CloseAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
        };

        List<CardAnimation> inventoryAnimations = new()
        {
            new ToInventoryAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
            new FromInventoryAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ),
        };

        _subLayouts = new()
        {
            new MainLayout(mainAnimations, _cardManager),
            new InventoryLayout(inventoryAnimations, _cardManager),
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

    public EnterInventoryPileAnimation InstantiateEnterInventoryPileAnimation()
    {
        return new EnterInventoryPileAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ);
    }

    public ExitInventoryPileAnimation InstantiateExitInventoryPileAnimation()
    {
        return new ExitInventoryPileAnimation(_cardManager, _waitTime, _horizontalTime, _verticalTime, _playSpaceBottomLeft, _playSpaceTopRight, TweenX, TweenY, TweenZ);
    }

    public void SetCardsRelativeToParent()
    {
        SetInventoryCardsRelativeToParent();
        SetMainCardsRelativeToParent();
    }

    public SubLayout CardTransitionToSubLayout(CardTransition transition)
    {
        switch (transition)
        {
            case CardTransition.CHILD:
            case CardTransition.BACK:
            case CardTransition.TOCOVER:
            case CardTransition.FROMCOVER:
            case CardTransition.CLOSE:
                return _subLayouts[0];
            case CardTransition.TOINVENTORY:
            case CardTransition.FROMINVENTORY:
                return _subLayouts[1];
        }

        throw new System.Exception("SubLayout not set");
    }

    public async Task SetLayoutBasedOnTransitionAnimated(CardNode activeNode, CardNode previousActiveNode, CardTransition transition)
    {
        _isAnimating = true;
        CardTransitionToSubLayout(transition).PrepareAnimation(transition);
        await CardTransitionToSubLayout(transition).CardTransitionToAnimation(transition).AnimateCards(activeNode, previousActiveNode, _cardManager.GetRootNode());
        CardTransitionToSubLayout(transition).FinishAnimation(transition);
        _isAnimating = false;
    }

    public void SetLayoutBasedOnTransitionStatic(CardNode activeNode, CardTransition transition)
    {
        CardTransitionToSubLayout(transition).PrepareStatic(transition);
        CardTransitionToSubLayout(transition).CardTransitionToAnimation(transition).MoveCardsStatic(activeNode, _cardManager.GetRootNode());
        CardTransitionToSubLayout(transition).FinishStatic(transition);
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

    public float GetBorder()
    {
        return _border;
    }
}