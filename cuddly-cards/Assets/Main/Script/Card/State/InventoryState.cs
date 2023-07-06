
using System.Linq;
using UnityEngine;
using static CardInfo;

public class InventoryState : LayoutState
{
    public InventoryState(CardManager cardManager) : base(cardManager)
    {
    }

    public override void StartState()
    {
        _animationManager.SetCardsStatic();
    }

    public override async void HandleClick(CardNode clickedNode, Click click)
    {
        if (clickedNode == null)
        {
            return;
        }

        if (click == Click.RIGHT)
        {
            _stateManager.PushState(new CloseUpState(_cardManager, clickedNode));
            return;
        }

        if (clickedNode.Context.CardType is CardType.KEY or CardType.DIALOGUE)
        {
            return;
        }

        if (clickedNode == _cardInventory.InventoryNode)
        {
            _animationManager.AddAnimation(new OpenAnimation(_cardManager));
            _animationManager.AddAnimation(new FromInventoryAnimation(_cardManager));

            await _animationManager.PlayAnimations(_cardManager.BaseNode);

            _stateManager.PopState();
            return;
        }


        EvaluateDefaultCardAction(clickedNode);
    }

    public async void EvaluateDefaultCardAction(CardNode clickedNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        CardNode previousActiveNode = _cardManager.BaseNode;

        LayoutState nextState;

        if (clickedNode == previousActiveNode)
        {
            _animationManager.AddAnimation(new OpenAnimation(_cardManager));
            _animationManager.AddAnimation(new FromInventoryAnimation(_cardManager));

            await _animationManager.PlayAnimations(_cardManager.BaseNode);

            _stateManager.PopState();
            return;
        }
        else if (previousActiveNode.Parent == clickedNode)
        {
            _animationManager.AddAnimation(new BackAnimation(_cardManager));
            _animationManager.AddAnimation(new FromInventoryAnimation(_cardManager));

            nextState = new MainState(_cardManager, clickedNode);

        }
        else if (clickedNode == rootNode)
        {
            _animationManager.AddAnimation(new ToCoverAnimation(_cardManager));
            _animationManager.AddAnimation(new FromInventoryAnimation(_cardManager));
            _animationManager.AddAnimation(new ExitInventoryPileAnimation(_cardManager));

            nextState = new CoverState(_cardManager, rootNode);
        }
        else
        {
            Debug.LogError("Pressed something weird");
            return;
        }

        await _animationManager.PlayAnimations(clickedNode, previousActiveNode);

        _stateManager.SetState(nextState);

    }

    public override void HandleHover(CardNode hoveredNode)
    {
        ResetHovers(hoveredNode);

        if (hoveredNode == null)
        {
            return;
        }

        if (hoveredNode.Context.CardType is not (CardType.KEY or CardType.DIALOGUE)) return;

        if (hoveredNode.Body.IsHovered) return;

        hoveredNode.Body.StartHoverTween();
    }

    public void ResetHovers(CardNode hoveredNode)
    {
        foreach (CardNode childNode in _cardInventory.InventoryNode.Children.SelectMany(partNode => partNode.Children))
        {
            childNode.Body.ResetHover(hoveredNode);
        }
    }
}
