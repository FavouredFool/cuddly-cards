using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using static CardInfo;

public class AnimationManager
{
    CardMover _cardMover;
    CardInputManager _cardInput;
    CardManager _cardManager;
    CardInventory _cardInventory;
    StateManager _stateManager;

    List<CardAnimation> _activeAnimations;
    List<CardAnimation> _allAnimations;
    List<SubLayout> _subLayouts;

    public AnimationManager(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = cardManager.CardMover;
        _cardInput = cardManager.CardInputManager;
        _cardInventory = cardManager.CardInventory;

        _stateManager = cardManager.StateManager;

        _allAnimations = _cardMover.GetCardAnimations();
        _subLayouts = _cardMover.GetSubLayouts();

        _activeAnimations = new();
    }

    public async Task PlayAnimations()
    {
        await PlayAnimations(null);
    }
    public async Task PlayAnimations(CardNode activeNode)
    {
        await PlayAnimations(activeNode, null);
    }
    public async Task PlayAnimations(CardNode activeNode, CardNode previousActiveNode)
    {
        await SetCardsAnimated(activeNode, previousActiveNode);
    }

    public async Task SetCardsAnimated(CardNode activeNode, CardNode previousActiveNode)
    {
        PrepareAnimation();

        Sequence allAnimations = DOTween.Sequence();

        foreach (CardAnimation animation in _activeAnimations)
        {
            allAnimations.Join(animation.GetAnimationSequence(activeNode, previousActiveNode));
        }

        await allAnimations.Play().OnComplete(() => { }).AsyncWaitForCompletion();

        FinishAnimation();
    }

    public void SetCardsStatic()
    {
        PrepareStatic();

        foreach (SubLayout subLayout in _subLayouts)
        {
            subLayout.SetLayoutStatic(_cardManager.BaseNode);
        }

        FinishStatic();
    }

    public void PrepareAnimation()
    {
        _cardInput.RemoveColliders();

        _cardManager.ClearTopLevelNodesMainPile();

        _cardMover.IsAnimatingFlag = true;
    }

    public void FinishAnimation()
    {
        _cardMover.IsAnimatingFlag = false;
    }

    public void PrepareStatic()
    {
        _cardInput.RemoveColliders();

        _cardManager.ClearTopLevelNodesMainPile();

        _cardMover.ResetPosition(_cardManager.RootNode);
    }

    public void FinishStatic()
    {
        _cardMover.SetHeightOfTopLevelNodes();

        _cardMover.SetHeightAndRotationOfInventory();

        _cardMover.SetMainCardsRelativeToParent();

        _cardMover.SetInventoryCardsRelativeToParent();

        _cardInput.SetColliders();

        ClearAnimations();
    }

    public void AddAnimation(CardTransition cardTransition)
    {
        _activeAnimations.Add(CardTransitionToAnimation(cardTransition));
    }

    public void ClearAnimations()
    {
        _activeAnimations.Clear();
    }

    public CardAnimation CardTransitionToAnimation(CardTransition transition)
    {
        return _allAnimations[(int)transition];
    }
}
