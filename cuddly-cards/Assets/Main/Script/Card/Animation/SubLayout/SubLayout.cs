using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubLayout
{
    protected CardManager _cardManager;
    protected CardMover _cardMover;
    protected CardInventory _cardInventory;
    protected StateManager _stateManager;

    public SubLayout(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = cardManager.GetCardMover();
        _cardInventory = cardManager.GetCardInventory();
        _stateManager = cardManager.GetStateManager();
    }

    public abstract void SetLayoutStatic(CardNode baseNode);
}
