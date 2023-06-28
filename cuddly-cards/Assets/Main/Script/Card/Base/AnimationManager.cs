using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using static CardInfo;

public class AnimationManager
{
    CardMover _cardMover;
    CardInput _cardInput;
    CardManager _cardManager;

    List<CardAnimation> _activeAnimations;
    List<CardAnimation> _allAnimations;

    public AnimationManager(CardManager cardManager, List<CardAnimation> animations)
    {
        _cardManager = cardManager;
        _cardMover = cardManager.GetCardMover();
        _cardInput = cardManager.GetCardInput();

        _allAnimations = animations;
        _activeAnimations = new();
    }

    public async Task PlayAnimations(bool isAnimated)
    {
        await PlayAnimations(null, isAnimated);
    }
    public async Task PlayAnimations(CardNode activeNode, bool isAnimated)
    {
        await PlayAnimations(activeNode, null, isAnimated);
    }
    public async Task PlayAnimations(CardNode activeNode, CardNode previousActiveNode, bool isAnimated)
    {
        if (isAnimated)
        {
            await SetCardsAnimated(activeNode, previousActiveNode);
        }
        else
        {
            SetCardsStatic(activeNode);
        }
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

        // Auf eine Animation wird immer statisch nochmal alles gesetzt
        SetCardsStatic(activeNode);
    }

    public void SetCardsStatic(CardNode activeNode)
    {
        PrepareStatic();

        // instead of moving everything statically separately - couldn't there be one big pile of predefined static positions that could be used?
        // - Cover position / mainPosition / dialoguePosition gets switched by baseNode type -> closed if inventory open
        // InventoryPosition gets added based on open or closed -> relative to cover its either on screen or offscreen
        // no going through the statics of each state (which overlaps completely anyway btw.)


        foreach (CardAnimation animation in _activeAnimations)
        {
            animation.MoveCardsStatic(activeNode);
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

        _cardMover.ResetPosition(_cardManager.GetRootNode());
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
