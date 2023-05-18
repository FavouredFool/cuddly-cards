using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public enum CardType { PLACE }

    CardBuilder _cardBuilder;
    CardMover _cardMover;
    CardInput _cardInput;

    CardNode _rootNode;

    List<CardNode> _topLevelNodes;

    public void Awake()
    {
        _topLevelNodes = new();

        _cardBuilder = GetComponent<CardBuilder>();
        _cardMover = GetComponent<CardMover>();
        _cardInput = GetComponent<CardInput>();
    }

    public void Start()
    {
        _rootNode = new(new("Se Beginning", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, first", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, second", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, third", CardType.PLACE));
        _rootNode[1].AddChild(new("Level 2, second_first", CardType.PLACE));
        _rootNode[1][0].AddChild(new("Level 3, first", CardType.PLACE));
        _rootNode[1][0].AddChild(new("Level 3, second", CardType.PLACE));
        _rootNode[1][0].AddChild(new("Level 3, third", CardType.PLACE));

        for (int i = 0; i < 100; i++)
        {
            _rootNode[1].AddChild(new("Extra", CardType.PLACE));
        }

        _cardBuilder.BuildAllCards(_rootNode);

        SetLayout(_rootNode);
    }


    public void SetLayout(CardNode mainNode)
    {
        ClearTopLevelNodes();

        SetTopNodes(mainNode);

        UpdatePiles();

        _cardMover.MoveCardsForLayout(mainNode, _rootNode);

        _cardInput.UpdateColliders();
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

}