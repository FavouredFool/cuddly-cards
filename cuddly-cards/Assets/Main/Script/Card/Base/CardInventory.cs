using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class CardInventory : MonoBehaviour
{
    CardNode _inventoryNode;

    public void InitializeInventory(CardBuilder builder)
    {
        _inventoryNode = new(new("Inventory", "lots of things in here", CardType.INVENTORY));
        _inventoryNode.Body = builder.BuildCardBody(_inventoryNode.Context);
        _inventoryNode.IsTopLevel = true;

        CardNode dialogueParentNode = new(new("Dialogue", "I need to talk about this.", CardType.INVENTORY));
        dialogueParentNode.Body = builder.BuildCardBody(dialogueParentNode.Context);

        CardNode keyParentNode = new(new("Keys", "All the things I have and know.", CardType.INVENTORY));
        keyParentNode.Body = builder.BuildCardBody(keyParentNode.Context);

        _inventoryNode.AddChild(dialogueParentNode);
        _inventoryNode.AddChild(keyParentNode);
        
    }

    public void AddNodeToInventory(CardNode node)
    {
        CardType type = node.Context.GetCardType();
        CardNode parentNode;

        if (type == CardType.DIALOGUE)
        {
            parentNode = _inventoryNode[0];
        }
        else if (type == CardType.KEY)
        {
            parentNode = _inventoryNode[1];
        }
        else
        {
            Debug.LogError("WRONG TYPE");
            return;
        }

        parentNode.AddChild(node);
    }

    public CardNode GetInventoryNode()
    {
        return _inventoryNode;
    }
}
