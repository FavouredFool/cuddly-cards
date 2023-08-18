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
        InventoryNode = new CardNode(new(-1, CardType.INVENTORY, "Inventory", "All the things I have and know", -1, -1, null));
        InventoryNode.Body = builder.BuildCardBody(InventoryNode.Context, InventoryNode, _cardManager.CardFolder);
        _cardManager.AddToTopLevel(InventoryNode);
    }

    public async void MoveKeyFromMainToInventory(CardNode node)
    {
        AnimationManager animationManager = _cardManager.AnimationManager;

        animationManager.AddAnimation(new CollectCardAnimation(_cardManager));
        await animationManager.PlayAnimations(node);

        node.UnlinkFromParent();

        AddKeyToInventory(node);

        animationManager.SetCardsStatic();
    }

    public void AddKeyToInventory(CardNode node)
    {
        InventoryNode.AddChild(node);
    }
}
