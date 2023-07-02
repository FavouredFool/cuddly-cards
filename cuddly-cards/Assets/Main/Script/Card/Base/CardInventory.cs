using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class CardInventory
{
    CardNode _inventoryNode;
    CardManager _cardManager;

    public CardInventory(CardManager cardManager)
    {
        _cardManager = cardManager;
    }

    public void InitializeInventory(CardBuilder builder)
    {
        _inventoryNode = new(new("Inventory", "lots of things in here", CardType.INVENTORY));
        _inventoryNode.Body = builder.BuildCardBody(_inventoryNode.Context, _cardManager.CardFolder);
        _inventoryNode.IsTopLevel = true;

        CardNode dialogueParentNode = new(new("Dialogue", "I need to talk about this.", CardType.INVENTORY));
        dialogueParentNode.Body = builder.BuildCardBody(dialogueParentNode.Context, _cardManager.CardFolder);

        CardNode keyParentNode = new(new("Keys", "All the things I have and know.", CardType.INVENTORY));
        keyParentNode.Body = builder.BuildCardBody(keyParentNode.Context, _cardManager.CardFolder);

        _inventoryNode.AddChild(dialogueParentNode);
        _inventoryNode.AddChild(keyParentNode);
    }

    public void MoveNodeFromMainToInventory(CardNode node)
    {
        // this would need to be done with animations and such. ALSO LOCK INPUTS DURING THIS TIME!

        // Remove node from main
        _cardManager.RemoveNodeFromMainNodes(node);
        AddNodeToInventory(node);

        // refresh everything
        _cardManager.AnimationManager.SetCardsStatic();
    }

    public void AddNodeToInventory(CardNode node)
    {
        CardNode parentNode;

        switch (node.Context.GetCardType())
        {
            case CardType.DIALOGUE:
                parentNode = _inventoryNode.Children[0];
                break;
            case CardType.KEY:
                parentNode = _inventoryNode.Children[1];
                break;
            default:
                throw new System.Exception("This CardType should not be added to the inventory");
        }

        parentNode.AddChild(node);
    }

    public CardNode GetInventoryNode()
    {
        return _inventoryNode;
    }
}
