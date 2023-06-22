using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class CardInventory : MonoBehaviour
{
    CardMover _cardMover;

    CardNode _inventoryNode;

    List<CardNode> _keyNodes;
    List<CardNode> _dialogueNodes;

    List<CardNode>[] _cardNodes;

    private void Awake()
    {
        _cardMover = GetComponent<CardMover>();
        _keyNodes = new();
        _dialogueNodes = new();
        _cardNodes = new[] { _keyNodes, _dialogueNodes };
    }

    public void InitializeInventory(CardBuilder builder)
    {
        _inventoryNode = new CardNode(new CardContext("Inventory", "lots of things in here", CardInfo.CardType.INVENTORY));

        _inventoryNode.Body = builder.BuildCardBody(_inventoryNode.Context);

        _cardMover.MoveCard(_inventoryNode, new Vector2(_cardMover.GetPlaySpaceTopRight().x + 1.5f, _cardMover.GetPlaySpaceBottomLeft().y));
    }

    public void AddNodeToInventory(CardNode node)
    {
        CardType type = node.Context.GetCardType();

        if (type == CardType.KEY)
        {
            Debug.Log("added key");
        }
        else if (type == CardType.DIALOGUE)
        {
            Debug.Log("added dialogue");
        }
        else
        {
            Debug.LogError("WRONG TYPE");
            return;
        }
    }
}
