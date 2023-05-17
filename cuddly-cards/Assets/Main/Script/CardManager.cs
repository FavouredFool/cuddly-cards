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

    ContextNode _rootContextNode;

    List<BodyNode> _topLevelNodes;

    public void Awake()
    {
        _topLevelNodes = new();
    }

    public void Start()
    {
        _rootContextNode = new(new("Se Beginning", CardType.PLACE));
        _rootContextNode.AddChild(new("Level 1, first", CardType.PLACE));
        _rootContextNode.AddChild(new("Level 1, second", CardType.PLACE));
        _rootContextNode.AddChild(new("Level 1, third", CardType.PLACE));
        _rootContextNode[1].AddChild(new("Level 2, second_first", CardType.PLACE));

        for (int i = 0; i < 100; i++)
        {
            _rootContextNode[1].AddChild(new("Extra", CardType.PLACE));
        }

        _cardBuilder.BuildAllCards(_rootContextNode);

        _topLevelNodes.Add(_rootContextNode.CBody);
        _topLevelNodes.Add(_rootContextNode[1].CBody);

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
        _cardMover.ParentCards(_rootContextNode, _topLevelNodes);

        foreach (BodyNode node in _topLevelNodes)
        {
            _cardMover.PileFromParenting(node);
        }
    }


}