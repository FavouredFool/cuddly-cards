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
            _topLevelNodes.ForEach(e => _cardMover.MoveCardRandom(e));

            _cardInput.UpdateColliders(_topLevelNodes);
        }
    }

    void UpdatePiles()
    {
        _cardMover.ParentCards(_rootNode, _topLevelNodes);

        foreach (CardNode node in _topLevelNodes)
        {
            _cardMover.PileFromParenting(node.Body);
        }
    }


}