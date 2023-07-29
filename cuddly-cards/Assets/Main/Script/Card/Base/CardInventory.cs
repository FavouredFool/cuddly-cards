using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class CardInventory
{
    public CardNode InventoryNode { get; set; }
    readonly CardManager _cardManager;

    public CardNode KeyParentNode { get; private set; }

    public CardInventory(CardManager cardManager)
    {
        _cardManager = cardManager;
    }

    public void InitializeInventory(CardBuilder builder)
    {
        InventoryNode = new CardNode(new CardContext("Inventory", "lots of things in here", CardType.INVENTORY));
        InventoryNode.Body = builder.BuildCardBody(InventoryNode.Context, InventoryNode, _cardManager.CardFolder);
        InventoryNode.IsTopLevel = true;

        KeyParentNode = new(new CardContext("Keys", "All the things I have and know.", CardType.INVENTORY));
        KeyParentNode.Body = builder.BuildCardBody(KeyParentNode.Context, KeyParentNode, _cardManager.CardFolder);

        InventoryNode.AddChild(KeyParentNode);
    }

    public async void MoveKeyFromMainToInventory(CardNode node)
    {
        AnimationManager animationManager = _cardManager.AnimationManager;

        animationManager.AddAnimation(new CollectCardAnimation(_cardManager));
        await animationManager.PlayAnimations(node);

        _cardManager.RemoveNodeFromMainNodes(node);
        AddKeyToInventory(node);

        animationManager.SetCardsStatic();
    }

    public void AddKeyToInventory(CardNode node)
    {
        KeyParentNode.AddChild(node);
    }
}
