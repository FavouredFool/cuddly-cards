using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using static CardInfo;

public class AnimationManager
{
    readonly CardMover _cardMover;
    readonly CardInputManager _cardInput;
    readonly CardManager _cardManager;

    readonly List<CardAnimation> _activeAnimations;
    readonly List<SubLayout> _subLayouts;

    public AnimationManager(CardManager cardManager)
    {
        _cardManager = cardManager;
        _cardMover = cardManager.CardMover;
        _cardInput = cardManager.CardInputManager;

        _subLayouts = _cardMover.GetSubLayouts();

        _activeAnimations = new List<CardAnimation>();
    }

    public async Task PlayAnimations(CardNode activeNode)
    {
        await PlayAnimations(activeNode, activeNode);
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

        // OnComplete is necessary because of a problem with Dotween
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

        ClearAnimations();
    }

    public void PrepareStatic()
    {
        _cardInput.RemoveColliders();

        _cardManager.ClearTopLevelNodesMainPile();

        _cardMover.ResetPosition(_cardManager.RootNode);
    }

    public void FinishStatic()
    {
        _cardMover.SetHeightAndRotationOfInventory();

        _cardMover.SetMainCardsRelativeToParent();

        _cardMover.SetInventoryCardsRelativeToParent();

        _cardInput.SetColliders();
    }

    public void AddAnimation(CardAnimation cardAnimation)
    {
        _activeAnimations.Add(cardAnimation);
    }

    public void ClearAnimations()
    {
        _activeAnimations.Clear();
    }
}
