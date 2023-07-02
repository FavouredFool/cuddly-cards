using UnityEngine;
using System.Collections.Generic;
using static CardInfo;
using System.Threading.Tasks;

public class CardManager : MonoBehaviour
{
    [Header("Story")]
    [SerializeField]
    TextAsset _textBlueprint;

    [Header("Organisation")]
    [SerializeField]
    Transform _cardFolder;

    [Header("Managers")]
    [SerializeField]
    CloseUpManager _closeUpManager;

    [SerializeField]
    CardInputManager _cardInputManager;

    [SerializeField]
    Camera _camera;


    public Transform CardFolder { get { return _cardFolder; } }
    public Camera Camera { get { return _camera; } }

    CardBuilder _cardBuilder;
    CardMover _cardMover;
    CardReader _cardReader;
    CardInventory _cardInventory;
    StateManager _stateManager;
    AnimationManager _animationManager;

    public CardBuilder CardBuilder { get { return _cardBuilder; } private set { _cardBuilder = value; } }
    public CardMover CardMover { get { return _cardMover; } private set { _cardMover = value; } }
    public CardInputManager CardInputManager { get { return _cardInputManager; } private set { _cardInputManager = value; } }
    public CardReader CardReader { get { return _cardReader; } private set { _cardReader = value; } }
    public CardInventory CardInventory { get { return _cardInventory; } private set { _cardInventory = value; } }
    public StateManager StateManager { get { return _stateManager; } private set { _stateManager = value; } }
    public CloseUpManager CloseUpManager { get { return _closeUpManager; } private set { _closeUpManager = value; } }
    public AnimationManager AnimationManager { get { return _animationManager; } private set { _animationManager = value; } }

    CardNode _rootNode;
    CardNode _baseNode;

    public CardNode RootNode { get { return _rootNode; } private set { _rootNode = value; } }
    public CardNode BaseNode { get { return _baseNode; } set { _baseNode = value; } }

    List<CardNode> _topLevelNodesMainPile;

    public void Awake()
    {
        _topLevelNodesMainPile = new();

        _cardBuilder = GetComponent<CardBuilder>();
        _cardMover = GetComponent<CardMover>();
        _cardMover.CardManager = this;
        _cardInputManager.CardManager = this;
        

        _cardReader = new CardReader(_textBlueprint);
        _stateManager = new StateManager(this);
        _cardInventory = new CardInventory(this);
        _animationManager = new AnimationManager(this);
    }

    public void Start()
    {
        _baseNode = _rootNode = _cardReader.ReadCards();

        _cardBuilder.BuildAllCards(_rootNode, _cardFolder);

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
            node.Body = _cardBuilder.BuildCardBody(node.Context, _cardFolder);
        }

        for (int i = 0; i < tests.Count; i++)
        {
            _cardInventory.AddNodeToInventory(tests[i]);
        }

        _stateManager.StartStates();
    }

    public void NodeHovered(CardNode hoveredNode)
    {
        if (hoveredNode == null)
        {
            return;
        }
        _stateManager.HandleHover(hoveredNode);
    }

    public void NodeClicked(CardNode clickedNode)
    {
        _stateManager.HandleClick(clickedNode);        
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
}