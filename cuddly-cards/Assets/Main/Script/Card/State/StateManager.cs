using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : StateMachine
{
    CardManager _cardManager;
    CardMover _cardMover;
    CardInventory _cardInventory;
    CardInput _cardInput;

    private void Awake()
    {
        _cardManager = GetComponent<CardManager>();
        _cardMover = GetComponent<CardMover>();
        _cardInventory = GetComponent<CardInventory>();
        _cardInput = GetComponent<CardInput>();
    }

    public void StartStates()
    {
        SetState(new CoverState(this));
    }

    public void HandleClick(CardNode clickedNode)
    {
        _state.HandleClick(clickedNode);
    }

    public CardManager GetCardManager()
    {
        return _cardManager;
    }
}
