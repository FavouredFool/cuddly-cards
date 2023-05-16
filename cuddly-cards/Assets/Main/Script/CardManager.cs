using UnityEngine;
using static CardNode;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    CardBuilder _cardBuilder;

    [SerializeField]
    CardMover _cardMover;

    public enum CardType { PLACE }

    CardNode _noteRoot;


    public void Start()
    {
        _noteRoot = new(new("Se Beginning", CardType.PLACE));
        _noteRoot.AddChild(new("Level 1, first", CardType.PLACE));
        _noteRoot.AddChild(new("Level 1, second", CardType.PLACE));
        _noteRoot.AddChild(new("Level 1, third", CardType.PLACE));
        _noteRoot[1].AddChild(new("Level 2, second_first", CardType.PLACE));


        _cardBuilder.BuildAllCards(_noteRoot);

        _cardMover.ParentCards(_noteRoot);


    }
}