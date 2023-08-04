using System;
using System.Collections.Generic;
using static CardInfo;

public abstract class SettedState : DefaultState
{
    protected readonly CardNode _newBaseNode;

    protected SettedState(CardManager cardManager, CardNode newBaseNode) : base(cardManager)
    {
        _newBaseNode = newBaseNode;
    }

    public override void StartState()
    {
        _cardManager.BaseNode = _newBaseNode;
        SetStatic();
    }
}
