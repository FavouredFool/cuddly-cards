using UnityEngine;
using System.Collections.Generic;
using static CardInfo;
using System.Threading.Tasks;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    CloseUpManager _closeUpManager;

    CardBuilder _cardBuilder;
    CardMover _cardMover;
    CardInput _cardInput;
    CardReader _cardReader;
    CardInventory _cardInventory;

    StateManager _stateManager;
    
    CardNode _rootNode;
    CardNode _baseNode;

    List<CardNode> _topLevelNodesMainPile;

    public void Awake()
    {
        _topLevelNodesMainPile = new();

        _cardBuilder = GetComponent<CardBuilder>();
        _cardMover = GetComponent<CardMover>();
        _cardInput = GetComponent<CardInput>();
        _cardReader = GetComponent<CardReader>();
        _cardInventory = GetComponent<CardInventory>();
        _stateManager = GetComponent<StateManager>();
    }

    public void Start()
    {
        _baseNode = _rootNode = _cardReader.ReadCards();

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

        _stateManager.StartStates();
    }

    public void NodeClicked(CardNode clickedNode)
    {
        _stateManager.HandleClick(clickedNode);        
    }

    public async Task SetMainLayoutBasedOnTransitionAnimated(CardNode clickedNode, CardNode previousBaseNode, CardTransition cardTransition)
    {
        _cardInput.RemoveColliders();

        ClearTopLevelNodesMainPile();

        await _cardMover.MoveMainCardsForLayoutAnimated(clickedNode, previousBaseNode, _rootNode, cardTransition);
    }

    public async Task SetInventoryLayoutBasedOnTransitionAnimated(CardTransition inventoryTransition)
    {
        _cardInput.RemoveColliders();

        await _cardMover.MoveInventoryCardsForLayoutAnimated(inventoryTransition);
    }

    public void SetInventoryLayoutBasedOnTransitionStatic(CardTransition inventoryTransition)
    {
        _cardInput.RemoveColliders();

        _cardMover.MoveInventoryCardsForLayoutStatic(inventoryTransition);

        _cardInput.SetColliders();
    }

    public void SetMainLayoutBasedOnTransitionStatic(CardNode clickedNode, CardTransition transition)
    {
        _cardInput.RemoveColliders();

        ClearTopLevelNodesMainPile();

        _cardMover.ResetPosition(GetRootNode());

        _cardMover.MoveMainCardsForLayoutStatic(clickedNode, GetRootNode(), transition);

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

    public void ClearTopLevelNodesMainPile()
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

    public CardNode GetBaseNode()
    {
        return _baseNode;
    }

    public void SetBaseNode(CardNode baseNode)
    {
        _baseNode = baseNode;
    }
}