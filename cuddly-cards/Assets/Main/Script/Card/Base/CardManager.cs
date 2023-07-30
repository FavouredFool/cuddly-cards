using UnityEngine;
using System.Collections.Generic;
using static CardInfo;
using System.Threading.Tasks;

public class CardManager : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] TextAsset _textBlueprint;

    [Header("Organization")]
    [SerializeField] Transform _cardFolder;

    [Header("Managers")]
    [SerializeField] CloseUpManager _closeUpManager;

    [SerializeField] CardInputManager _cardInputManager;

    [SerializeField] Camera _camera;

    public Transform CardFolder => _cardFolder;
    public Camera Camera => _camera;
    public CardInputManager CardInputManager { get => _cardInputManager; private set => _cardInputManager = value; }
    public CloseUpManager CloseUpManager { get => _closeUpManager; private set => _closeUpManager = value; }

    public CardBuilder CardBuilder { get; private set; }
    public CardMover CardMover { get; private set; }
    public CardReader CardReader { get; private set; }
    public CardInventory CardInventory { get; private set; }
    public CardDialogue CardDialogue { get; private set; }
    public StateManager StateManager { get; private set; }
    public AnimationManager AnimationManager { get; private set; }
    public CardNode RootNode { get; private set; }
    public CardNode BaseNode { get; set; }


    List<CardNode> _topLevelNodesMainPile;

    public void Awake()
    {
        _topLevelNodesMainPile = new List<CardNode>();

        CardBuilder = GetComponent<CardBuilder>();
        CardMover = GetComponent<CardMover>();
        CardMover.CardManager = this;
        _cardInputManager.CardManager = this;

        CardReader = new CardReader(_textBlueprint);
        StateManager = new StateManager(this);
        CardInventory = new CardInventory(this);
        CardDialogue = new CardDialogue(this);
        AnimationManager = new AnimationManager(this);
    }

    public void Start()
    {
        BaseNode = RootNode = CardReader.ReadCards();

        CardBuilder.BuildAllCards(RootNode, _cardFolder);

        CardInventory.InitializeInventory(CardBuilder);

        CreateLocksAndDialogueOnStartUp();

        StateManager.StartStates();
    }

    public void CreateLocksAndDialogueOnStartUp()
    {
        List<CardNode> tests = new();

        for (int i = 0; i < 5; i++)
        {
            tests.Add(new CardNode(new CardContext("Dagger", "sharp like... a dagger i guess", CardInfo.CardType.KEY)));
            tests.Add(new CardNode(new CardContext("A revelation", "you just had a dangerous thought", CardInfo.CardType.KEY)));
        }

        foreach (CardNode node in tests)
        {
            node.Body = CardBuilder.BuildCardBody(node.Context, node, _cardFolder);
        }

        foreach (CardNode cardNode in tests)
        {
            CardInventory.AddKeyToInventory(cardNode);
        }
    }

    public void NodeHovered(CardNode hoveredNode)
    {
        StateManager.HandleHover(hoveredNode);
    }

    public void NodeClicked(CardNode clickedNode, Click click)
    {
        StateManager.HandleClick(clickedNode, click);        
    }

    public List<CardNode> GetClickableNodes()
    {
        List<CardNode> clickables = new(GetTopLevelNodesMainPile()) { CardInventory.InventoryNode };

        return clickables;
    }

    public void ClearTopLevelNodesMainPile()
    {
        _topLevelNodesMainPile.Clear();

        RootNode.TraverseChildren(CardInfo.CardTraversal.CONTEXT,
            delegate (CardNode node)
        {
            node.IsTopLevel = false;
            return true;
        });

        foreach (CardNode node in CardInventory.InventoryNode.Children)
        {
            node.IsTopLevel = false;
        }
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