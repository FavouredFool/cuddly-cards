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

    List<CardNode> _topLevelNodes;

    bool _isCloseUp = false;

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
        _activeNode = _rootNode = _cardReader.ReadCards();

        _cardBuilder.BuildAllCards(_rootNode);

        SetLayout();
    }

    public void SetLayout()
    {
        ClearTopLevelNodes();

        SetTopNodes(_activeNode);

        UpdatePiles();

        _cardMover.MoveCardsForLayout(_activeNode, _rootNode);

        _cardInput.UpdateColliders();
    }

    public void EnterCloseUp(CardNode closeUpNode)
    {
        _isCloseUp = true;
        _closeUpManager.EnterCloseUp(closeUpNode);
    }

    public void ExitCloseUp()
    {
        _isCloseUp = false;
        _closeUpManager.ExitCloseUp();
    }

    void SetTopNodes(CardNode mainNode)
    {
        _topLevelNodes.Add(_rootNode);

        if (mainNode != _rootNode)
        {
            _topLevelNodes.Add(mainNode);

            if (mainNode.Parent != _rootNode)
            {
                _topLevelNodes.Add(mainNode.Parent);
            }
        }

        foreach (CardNode childNode in mainNode.Children)
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

    void UpdatePiles()
    {
        RefreshTopLevelForAllNodes();

        _cardMover.ParentCards(_rootNode, _topLevelNodes);

        _cardMover.ResetPositionAndRotation(_rootNode);

        foreach (CardNode node in _topLevelNodes)
        {
            _cardMover.PileFromParenting(node);
        }
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

    public void SetActiveNode(CardNode activeNode)
    {
        _activeNode = activeNode;
    }

    public bool GetIsCloseUp()
    {
        return _isCloseUp;
    }


}