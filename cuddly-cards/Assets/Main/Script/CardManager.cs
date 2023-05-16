using UnityEngine;
using static CardNode;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    CardBuilder _cardBuilder;

    [SerializeField]
    CardMover _cardMover;

    public enum CardType { PLACE }

    CardNode _rootNode;


    public void Start()
    {
        _rootNode = new(new("Se Beginning", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, first", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, second", CardType.PLACE));
        _rootNode.AddChild(new("Level 1, third", CardType.PLACE));
        _rootNode[1].AddChild(new("Level 2, second_first", CardType.PLACE));


        _cardBuilder.BuildAllCards(_rootNode);

        _cardMover.ParentCards(_rootNode);

        //_cardMover.PileFromParenting(_rootNode);

        _cardMover.PileFromParentingTransform(_rootNode.Body);


    }
}