using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    CloseUpManager _closeUpManager;

    CardBuilder _cardBuilder;
    CardMover _cardMover;
    CardInput _cardInput;
    CardReader _cardReader;
    
    CardNode _rootNode;
    CardNode _activeNode;
    CardNode _oldActiveNode;

    List<CardNode> _topLevelNodes;

    bool _isCloseUp = false;

    bool _isStartLayout = false;

    public void Awake()
    {
        _topLevelNodes = new();

        _cardBuilder = GetComponent<CardBuilder>();
        _cardMover = GetComponent<CardMover>();
        _cardInput = GetComponent<CardInput>();
        _cardReader = GetComponent<CardReader>();
    }

    public void Start()
    {
        _activeNode = _oldActiveNode = _rootNode = _cardReader.ReadCards();

        _cardBuilder.BuildAllCards(_rootNode);

        FinishStartLayout();
    }

    public void SetNodeActive(CardNode node)
    {
        _oldActiveNode = _activeNode;
        _activeNode = node;

        if (_activeNode.Context.GetHasBeenSeen())
        {
            PrepareLayout();
        }
        else
        {
            EnterCloseUp();
        }
    }

    public void PrepareLayout()
    {
        _cardInput.RemoveColliders();

        ClearTopLevelNodes();
        SetTopNodes();
        RefreshTopLevelForAllNodes();

        _cardMover.RemoveParenting(_rootNode);

        // Extra for initial layout
        if (_activeNode == _rootNode)
        {
            // aber nur wenn's vorher nicht rootnode war!!
            if (_rootNode != _oldActiveNode && _rootNode != _oldActiveNode.Parent)
            {
                _cardMover.MoveCardsForStartLayoutAnimated(_rootNode, _oldActiveNode);
                return;
            }
        }

        _cardMover.MoveCardsForLayoutAnimated(_activeNode, _oldActiveNode, _rootNode, _isStartLayout);
    }

    public void FinishLayout()
    {
        _isStartLayout = false;

        ClearTopLevelNodes();
        SetTopNodes();
        RefreshTopLevelForAllNodes();
        _cardMover.ResetPosition(_rootNode);

        _cardMover.ParentCards(_rootNode);
        _cardMover.PileFromParenting(_topLevelNodes);

        _cardMover.MoveCardsForLayoutStatic(_activeNode, _oldActiveNode, _rootNode);

        _cardInput.SetColliders();
    }

    public void FinishStartLayout()
    {
        _isStartLayout = true;

        ClearTopLevelNodes();
        // children are not top-level in this one!
        _topLevelNodes.Add(_rootNode);
        RefreshTopLevelForAllNodes();
        _cardMover.ResetPosition(_rootNode);

        _cardMover.ParentCards(_rootNode);
        _cardMover.PileFromParenting(_topLevelNodes);

        _cardMover.MoveCardsForStartLayoutStatic(_rootNode);

        _cardInput.SetColliders();
    }

    public void EnterCloseUp()
    {
        _closeUpManager.EnterCloseUp(_activeNode);

        _isCloseUp = true;
        _cardInput.RemoveColliders();
    }

    public void ExitCloseUp()
    {
        if (!_closeUpManager.GetEnterAnimationFinished())
        {
            return;
        }

        _isCloseUp = false;
        _closeUpManager.ExitCloseUp();
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

    void SetTopNodes()
    {
        _topLevelNodes.Add(_rootNode);

        if (_activeNode != _rootNode)
        {
            _topLevelNodes.Add(_activeNode);

            if (_activeNode.Parent != _rootNode)
            {
                _topLevelNodes.Add(_activeNode.Parent);
            }
        }

        foreach (CardNode childNode in _activeNode.Children)
        {
            _topLevelNodes.Add(childNode);
        }
    }

    void ClearTopLevelNodes()
    {
        _topLevelNodes.Clear();
    }

    void RefreshTopLevelForAllNodes()
    {
        _rootNode.TraverseContext(
            delegate (CardNode node)
        {
            node.IsTopLevel = _topLevelNodes.Contains(node);
            return true;
        });
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

    public bool GetIsStartLayout()
    {
        return _isStartLayout;
    }

    public bool GetIsCloseUp()
    {
        return _isCloseUp;
    }

    public void AddToTopLevel(CardNode cardNode)
    {
        _topLevelNodes.Add(cardNode);
        cardNode.IsTopLevel = true;
    }

    public void RemoveFromTopLevel(CardNode cardNode)
    {
        _topLevelNodes.Remove(cardNode);
        cardNode.IsTopLevel = false;
    }
}