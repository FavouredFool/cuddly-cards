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
        List<CardNode> keys = new();

        for (int i = 0; i < 5; i++)
        {
            keys.Add(new CardNode(new CardContext("Dagger", "sharp like... a dagger i guess", CardInfo.CardType.KEY)));
            keys.Add(new CardNode(new CardContext("A revelation", "you just had a dangerous thought", CardInfo.CardType.KEY)));
        }

        foreach (CardNode node in keys)
        {
            node.Body = CardBuilder.BuildCardBody(node.Context, node, _cardFolder);
        }

        foreach (CardNode cardNode in keys)
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
        List<CardNode> clickables = new();
        foreach (CardNode topLevelNode in GetTopLevelNodes())
        {
            if (!topLevelNode.IsClickable)
            {
                continue;
            }

            clickables.Add(topLevelNode);
        }

        return clickables;
    }

    public void ClearTopLevelNodesMainPile()
    {
        _topLevelNodesMainPile.Clear();

        // reset toplevel of main
        RootNode.TraverseChildren(CardInfo.CardTraversal.CONTEXT,
            delegate (CardNode node)
        {
            node.SetNodeState(false, false);
            return true;
        });

        // reset toplevel of inventory
        foreach (CardNode node in CardInventory.InventoryNode.Children)
        {
            node.SetNodeState(false, false);
        }
    }

    public CardNode GetCardNodeFromID(int id)
    {
        CardNode foundNode = null;

        RootNode.TraverseChildren(CardInfo.CardTraversal.CONTEXT,
            delegate (CardNode node)
            {
                if (node.Context.ID == id)
                {
                    foundNode = node;
                }

                return true;
            });

        if (foundNode == null)
        {
            throw new System.Exception("Found no node with ID: " + id);
        }

        return foundNode;
    }

    public void AddToTopLevel(CardNode cardNode)
    {
        AddToTopLevel(cardNode, true);
    }

    public void AddToTopLevel(CardNode cardNode, bool isClickable)
    {
        if (_topLevelNodesMainPile.Contains(cardNode))
        {
            Debug.LogWarning("Toplevels versucht zu doppeln");
            return;
        }

        _topLevelNodesMainPile.Add(cardNode);
        cardNode.SetNodeState(true, isClickable);
    }

    public List<CardNode> GetTopLevelNodes()
    {
        return _topLevelNodesMainPile;
    }
}