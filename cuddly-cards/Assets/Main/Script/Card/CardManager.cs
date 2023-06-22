using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    CloseUpManager _closeUpManager;

    CardBuilder _cardBuilder;
    CardMover _cardMover;
    CardInput _cardInput;
    CardReader _cardReader;
    CardInventory _cardInventory;
    
    CardNode _rootNode;
    CardNode _activeNode;
    CardNode _oldActiveNode;

    List<CardNode> _topLevelNodesMainPile;

    bool _isCloseUp = false;
    public bool IsCloseUpFlag { get { return _isCloseUp; } set { _isCloseUp = value; } }

    bool _isStartLayout = false;
    public bool IsStartLayoutFlag { get { return _isStartLayout; } set { _isStartLayout = value; } }

    public void Awake()
    {
        _topLevelNodesMainPile = new();

        _cardBuilder = GetComponent<CardBuilder>();
        _cardMover = GetComponent<CardMover>();
        _cardInput = GetComponent<CardInput>();
        _cardReader = GetComponent<CardReader>();
        _cardInventory = GetComponent<CardInventory>();
    }

    public void Start()
    {
        _activeNode = _oldActiveNode = _rootNode = _cardReader.ReadCards();

        _cardBuilder.BuildAllCards(_rootNode);

        _cardInventory.InitializeInventory(_cardBuilder);

        List<CardNode> tests = new();
        for (int i = 0; i < 5; i++)
        {
            tests.Add(new CardNode(new CardContext("Dagger", "sharp like... a dagger i guess", CardInfo.CardType.KEY)));
            tests.Add(new CardNode(new CardContext("A revelation", "you just had a dangerous thought", CardInfo.CardType.KEY)));
        }

        for (int i = 0; i < 2; i++)
        {
            tests.Add(new CardNode(new CardContext("The affair", "Not a nice topic to talk about. Don't expect a happy welcome.", CardInfo.CardType.DIALOGUE)));
            tests.Add(new CardNode(new CardContext("Bad friends", "The worst.", CardInfo.CardType.DIALOGUE)));
        }

        foreach (CardNode node in tests)
        {
            node.Body = _cardBuilder.BuildCardBody(node.Context);
        }

        for (int i = 0; i < tests.Count; i++)
        {
            _cardInventory.AddNodeToInventory(tests[i]);
        }

        FinishLayout(true);
    }

    public void NodeClicked(CardNode clickedNode)
    {
        switch (clickedNode.Context.GetCardType())
        {
            case CardType.INVENTORY:
                InventoryNodeAction();
                return;
            case CardType.KEY:
            case CardType.DIALOGUE:
                return;
            case CardType.LOCK:
                return;
            default:
                DefaultNodeAction(clickedNode);
                return;
        }
        
    }

    void DefaultNodeAction(CardNode clickedNode)
    {
        if (clickedNode == _activeNode && !IsStartLayoutFlag)
        {
            EnterCloseUp();
        }
        else
        {
            SetNodeActive(clickedNode);
        }
    }

    void InventoryNodeAction()
    {
        if (_cardInventory.InventoryIsOpenFlag)
        {
            _cardInventory.InventoryShouldOpenFlag = false;

        }
        else
        {
            _cardInventory.InventoryShouldOpenFlag = true;
        }

        PrepareLayout();
    }

    public void SetNodeActive(CardNode node)
    {
        _oldActiveNode = _activeNode;
        _activeNode = node;

        if (!_activeNode.Context.GetHasBeenSeen())
        {
            EnterCloseUp();
        }
        else
        {
            PrepareLayout();
        }
    }

    public void EnterCloseUp()
    {
        _closeUpManager.EnterCloseUp(_activeNode);

        IsCloseUpFlag = true;
        _cardInput.RemoveColliders();
    }

    public void ExitCloseUp()
    {
        if (!_closeUpManager.IntroAnimationFinishedFlag)
        {
            return;
        }

        _closeUpManager.ExitCloseUp();
        _isCloseUp = false;
    }

    public void CloseUpFinished(bool initialCloseUp)
    {
        if (initialCloseUp)
        {
            PrepareLayout();
        }
        else
        {
            _cardInput.SetColliders();
        }
    }

    public void PrepareLayout()
    {
        _cardInput.RemoveColliders();

        ClearTopLevelNodesMainPile();

        // check if Inventory has changed
        if (_cardInventory.InventoryShouldOpenFlag != _cardInventory.InventoryIsOpenFlag)
        {
            _cardMover.MoveCardsForToggleInventoryAnimated();
            return;
        }

        bool activateStartLayout = false;

        if (_activeNode == _rootNode)
        {
            if (_rootNode != _oldActiveNode && _rootNode != _oldActiveNode.Parent)
            {
                activateStartLayout = true;
            }
        }

        _cardMover.MoveCardsForLayoutAnimated(_activeNode, _oldActiveNode, _rootNode, activateStartLayout);
    }



    public void FinishLayout(bool isStartLayout)
    {
        IsStartLayoutFlag = isStartLayout;

        ClearTopLevelNodesMainPile();

        _cardMover.ResetPosition(_rootNode);

        _cardMover.MoveCardsForLayoutStatic(_activeNode, _rootNode, isStartLayout);

        _cardMover.SetHeights();

        _cardMover.SetCardsRelativeToParent();

        _cardInput.SetColliders();

        _cardInventory.InventoryIsOpenFlag = _cardInventory.InventoryShouldOpenFlag;
    }

    public List<CardNode> GetClickableNodes()
    {
        List<CardNode> clickables = new(GetTopLevelNodesMainPile());
        clickables.Add(_cardInventory.GetInventoryNode());

        return clickables;
    }

    void ClearTopLevelNodesMainPile()
    {
        _topLevelNodesMainPile.Clear();

        _rootNode.TraverseChildren(CardInfo.CardTraversal.CONTEXT,
            delegate (CardNode node)
        {
            node.IsTopLevel = _topLevelNodesMainPile.Contains(node);
            return true;
        });
    }

    public void AddToTopLevelMainPile(CardNode cardNode)
    {
        _topLevelNodesMainPile.Add(cardNode);
        cardNode.IsTopLevel = true;
    }

    public List<CardNode> GetTopLevelNodesMainPile()
    {
        return _topLevelNodesMainPile;
    }

    public CardNode GetRootNode()
    {
        return _rootNode;
    }

    public CardNode GetActiveNode()
    {
        return _activeNode;
    }

    public CardNode GetOldActiveNode()
    {
        return _oldActiveNode;
    }
}