using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubLayout
{
    protected CardManager _cardManager;
    protected CardMover _cardMover;
    protected CardInventory _cardInventory;
    protected StateManager _stateManager;
    protected SubStaticPositions _subStatics;

    protected SubLayout(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = cardManager.CardMover;
        _cardInventory = cardManager.CardInventory;
        _stateManager = cardManager.StateManager;
        _subStatics = cardManager.CardMover.SubStatics;
    }

    public abstract void SetLayoutStatic(CardNode baseNode);
}
