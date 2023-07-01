using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : StateMachine
{
    [SerializeField]
    CloseUpManager _closeUpManager;

    CardManager _cardManager;
    CardMover _cardMover;
    CardInventory _cardInventory;
    CardInput _cardInput;

    private new void Awake()
    {
        base.Awake();

        _cardManager = GetComponent<CardManager>();
        _cardMover = GetComponent<CardMover>();
        _cardInventory = GetComponent<CardInventory>();
        _cardInput = GetComponent<CardInput>();
    }

    public void StartStates()
    {
        SetState(new CoverState(this));
    }

    internal void HandleHover(CardNode hoveredNode)
    {
        _states.Peek().HandleHover(hoveredNode);
    }

    public void HandleClick(CardNode clickedNode)
    {
        _states.Peek().HandleClick(clickedNode);
    }

    public CardManager GetCardManager()
    {
        return _cardManager;
    }

    public CloseUpManager GetCloseUpManager()
    {
        return _closeUpManager;
    }

    public AnimationManager GetAnimationManager()
    {
        return _cardMover.GetAnimationManager();
    }
}
