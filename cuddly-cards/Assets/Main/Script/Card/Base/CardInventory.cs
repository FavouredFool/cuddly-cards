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
        _inventoryNode = new CardNode(new CardContext("Inventory", "lots of things in here", CardType.INVENTORY));
        _inventoryNode.Body = builder.BuildCardBody(_inventoryNode.Context, _cardManager.CardFolder);
        _inventoryNode.IsTopLevel = true;

        CardNode dialogueParentNode = new(new CardContext("Dialogue", "I need to talk about this.", CardType.INVENTORY));
        dialogueParentNode.Body = builder.BuildCardBody(dialogueParentNode.Context, _cardManager.CardFolder);

        CardNode keyParentNode = new(new CardContext("Keys", "All the things I have and know.", CardType.INVENTORY));
        keyParentNode.Body = builder.BuildCardBody(keyParentNode.Context, _cardManager.CardFolder);

        _inventoryNode.AddChild(dialogueParentNode);
        _inventoryNode.AddChild(keyParentNode);
    }

    public async void MoveNodeFromMainToInventory(CardNode node)
    {
        AnimationManager animationManager = _cardManager.AnimationManager;

        animationManager.AddAnimation(new CollectCardAnimation(_cardManager));
        await animationManager.PlayAnimations(node);

        _cardManager.RemoveNodeFromMainNodes(node);
        AddNodeToInventory(node);

        animationManager.SetCardsStatic();
    }

    public void AddNodeToInventory(CardNode node)
    {
        CardNode parentNode = node.Context.CardType switch
        {
            CardType.DIALOGUE => _inventoryNode.Children[0],
            CardType.KEY => _inventoryNode.Children[1],
            _ => throw new System.Exception("This CardType should not be added to the inventory")
        };

        parentNode.AddChild(node);
    }

    public CardNode GetInventoryNode()
    {
        return _inventoryNode;
    }
}
