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

        FinishLayout(true);
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
        _cardMover.RemoveParenting(_rootNode);

        bool activateStartLayout = false;

        // Extra Logic that enables having a "cover" cardpile, to which cards can be moved back
        if (_activeNode == _rootNode)
        {
            if (_rootNode != _oldActiveNode && _rootNode != _oldActiveNode.Parent)
            {
                activateStartLayout = true;
            }
        }

        ClearAndRefreshTopLevelNodes();

        _cardMover.MoveCardsForLayoutAnimated(_activeNode, _oldActiveNode, _rootNode, _isStartLayout, activateStartLayout);
    }

    public void FinishLayout(bool isStartLayout)
    {
        _isStartLayout = isStartLayout;

        ClearAndRefreshTopLevelNodes();

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

    void ClearAndRefreshTopLevelNodes()
    {
        ClearTopLevelNodes();
        RefreshTopLevelForAllNodes();
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
}