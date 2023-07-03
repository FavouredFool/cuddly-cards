
using UnityEngine;
using static CardInfo;

public class LockState : LayoutState
{
    CardNode _baseNode;

    public LockState(CardManager cardManager, CardNode baseNode) : base (cardManager)
    {
        _baseNode = baseNode;
    }

    public override void StartState()
    {
        _cardManager.BaseNode = _baseNode;
        _animationManager.SetCardsStatic();
    }

    public override void HandleClick(CardNode clickedNode)
    {
        if (clickedNode == null)
        {
            return;
        }

        CardType cardType = clickedNode.Context.GetCardType();

        switch (cardType)
        {
            case CardType.INVENTORY:
                return;

            case CardType.KEY:
            case CardType.DIALOGUE:
                _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, false));
                return;

            default:
                EvaluateDefaultCardAction(clickedNode);
                return;
        }
    }

    public async void EvaluateDefaultCardAction(CardNode clickedNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        CardNode previousActiveNode = _cardManager.BaseNode;

        CardInfo.CardTransition cardTransition;
        LayoutState nextState;

        if (clickedNode == previousActiveNode)
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode, false));
            return;
        }
        else if (previousActiveNode.Parent == clickedNode)
        {
            // Inventar muss eingefahren werden -> Neue Transition
            // pressed back

            _animationManager.AddAnimation(CardTransition.BACK);
            // modular?
            //_animationManager.AddAnimation(CardTransition.OPEN);

            await _animationManager.PlayAnimations(clickedNode, previousActiveNode);

            _stateManager.SetState(new MainState(_cardManager, clickedNode));
            return;
        }
        else if (clickedNode == rootNode)
        {
            //_animationManager.AddAnimation(CardTransition.TOCOVER);

            //_animationManager.AddAnimation(CardTransition.OPEN);

            //await _animationManager.PlayAnimations(clickedNode, previousActiveNode);
            //_stateManager.SetState(new CoverState(_cardManager));

            return;
        }
        else
        {
            Debug.LogError("Pressed something weird");
            return;
        }
    }

    public override void HandleHover(CardNode hoveredNode)
    {
        return;
    }
}
