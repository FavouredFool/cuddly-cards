using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    CardBuilder _cardBuilder;

    [SerializeField]
    CardMover _cardMover;

    [SerializeField]
    CardInput _cardInput;

    public enum CardType { PLACE }

    CardNode _rootNode;

    List<CardNode> _topLevelNodes;

    public void Awake()
    {
        _topLevelNodes = new();
    }

    public void Start()
    {
        _rootNode = new(new("Se Beginning", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, first", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, second", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, third", CardType.PLACE));
        _rootNode[1].AddChild(new("Level 2, second_first", CardType.PLACE));

        for (int i = 0; i < 100; i++)
        {
            _rootNode[1].AddChild(new("Extra", CardType.PLACE));
        }

        _cardBuilder.BuildAllCards(_rootNode);

        _topLevelNodes.Add(_rootNode);
        _topLevelNodes.Add(_rootNode[1]);

        UpdatePiles();

        _topLevelNodes.ForEach(e => _cardMover.MoveCardRandom(e));

        _cardInput.UpdateColliders(_topLevelNodes);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetLayout(_rootNode);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetLayout(_rootNode[1]);
        }
    }

    public void SetLayout(CardNode mainNode)
    {
        // When a cardpile is pressed, it's layout is set. This involves:
        // Moving the topmost card onto a specific position
        // Making direct children to topmost cards, reparenting, moving them based on their amount, putting all other cards someplace else
        // updating colliders


        // make all topcards false
        ClearTopLevelNodes();

        // set topcards
        if (mainNode != _rootNode)
        {
            _topLevelNodes.Add(_rootNode);
        }

        _topLevelNodes.Add(mainNode);

        foreach (CardNode childNode in mainNode.Children)
        {
            _topLevelNodes.Add(childNode);
        }

        // update the entire pile (always necessary)
        UpdatePiles();

        if (mainNode != _rootNode)
        {
            _cardMover.MoveCard(_rootNode, new Vector2(3, 3));
        }

        // Move them
        _cardMover.MoveCard(mainNode, new Vector2(-3f, 0));

        for (int i = 0; i < mainNode.Children.Count; i++)
        {
            _cardMover.MoveCard(mainNode.Children[i], new Vector2(i * 1.5f - 1f, 0));
        }

        _cardInput.UpdateColliders(_topLevelNodes);


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

        foreach (CardNode node in _topLevelNodes)
        {
            _cardMover.PileFromParenting(node);
        }
    }

    public List<CardNode> GetTopLevelNodes()
    {
        return _topLevelNodes;
    }

}