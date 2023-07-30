using UnityEngine;
using System.Collections.Generic;
using static CardInfo;

public class CardInventory
{
    public CardNode InventoryNode { get; set; }
    readonly CardManager _cardManager;

    public CardInventory(CardManager cardManager)
    {
        _cardManager = cardManager;
    }

    public void InitializeInventory(CardBuilder builder)
    {
        InventoryNode = new CardNode(new CardContext("Inventory", "All the things I have and know.", CardType.INVENTORY));
        InventoryNode.Body = builder.BuildCardBody(InventoryNode.Context, InventoryNode, _cardManager.CardFolder);
        InventoryNode.IsTopLevel = true;
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
        InventoryNode.AddChild(node);
    }
}
