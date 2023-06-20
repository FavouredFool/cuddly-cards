using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    CloseUpManager _closeUpManager;

    [SerializeField]
    ModelRenderManager _renderManager;

    [SerializeField]
    List<CardNode> _initialNodes;

    CardBuilder _cardBuilder;
    CardMover _cardMover;
    CardInput _cardInput;
    CardReader _cardReader;
    
    CardNode _rootNode;
    CardNode _activeNode;
    CardNode _oldActiveNode;

    List<CardNode> _topLevelNodes;
    

    bool _isCloseUp = false;
    public bool IsCloseUpFlag { get { return _isCloseUp; } set { _isCloseUp = value; } }

    bool _isStartLayout = false;
    public bool IsStartLayoutFlag { get { return _isStartLayout; } set { _isStartLayout = value; } }

    public void Awake()
    {
        _topLevelNodes = new();
        _initialNodes = new();

        _cardBuilder = GetComponent<CardBuilder>();
        _cardMover = GetComponent<CardMover>();
        _cardInput = GetComponent<CardInput>();
        _cardReader = GetComponent<CardReader>();
    }

    public void Start()
    {
        _activeNode = _oldActiveNode = _rootNode = _cardReader.ReadCards();

        _cardBuilder.SetModelForCardContexts(_rootNode);
        _cardBuilder.BuildAllCards(_rootNode);

        FinishLayout(true);

        foreach (CardNode node in _initialNodes)
        {
            AddToTopLevel(node);
        }
    }

    public void NodeClicked(CardNode clickedNode)
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

        ClearTopLevelNodes();

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

        ClearTopLevelNodes();

        _cardMover.ResetPosition(_rootNode);

        if (isStartLayout)
        {
            AddToTopLevel(_rootNode);
            _cardMover.MoveCardsForStartLayoutStatic(_rootNode);
        }
        else
        {
            _cardMover.MoveCardsForLayoutStatic(_activeNode, _rootNode);
        }

        _cardMover.SetHeightOfTopLevelNodes();

        _cardMover.SetCardsRelativeToParent();

        _cardInput.SetColliders();
    }

    void ClearTopLevelNodes()
    {
        _topLevelNodes.Clear();

        _rootNode.TraverseChildren(CardInfo.CardTraversal.CONTEXT,
            delegate (CardNode node)
        {
            node.IsTopLevel = _topLevelNodes.Contains(node);
            return true;
        });

        // Reset Model
        _renderManager.ResetModels();
    }

    public void AddToTopLevel(CardNode cardNode)
    {
        _topLevelNodes.Add(cardNode);
        cardNode.IsTopLevel = true;

        _renderManager.SetModel(cardNode);
    }

    public List<CardNode> GetTopLevelNodes()
    {
        return _topLevelNodes;
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