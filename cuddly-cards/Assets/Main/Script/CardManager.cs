using UnityEngine;
using static TreeNode<CardContext>;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    CardBuilder _cardBuilder;

    [SerializeField]
    CardMover _cardMover;

    public enum CardType { PLACE }

    TreeNode<CardContext> _treeRoot;


    public void Start()
    {
        _treeRoot = new(new("Se Beginning", CardType.PLACE));
        _treeRoot.AddChild(new("Level 1, first", CardType.PLACE));
        _treeRoot.AddChild(new("Level 1, second", CardType.PLACE));
        _treeRoot.AddChild(new("Level 1, third", CardType.PLACE));
        _treeRoot[1].AddChild(new("Level 2, second_first", CardType.PLACE));


        _cardBuilder.BuildAllCards(_treeRoot);

        _cardMover.ParentCards(_treeRoot);


    }
}