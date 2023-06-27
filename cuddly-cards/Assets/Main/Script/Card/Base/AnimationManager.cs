using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    CardMover _cardMover;
    CardInput _cardInput;
    CardManager _cardManager;

    List<CardAnimation> _animations;

    public void Awake()
    {
        _cardMover = GetComponent<CardMover>();
        _cardInput = GetComponent<CardInput>();
        _cardManager = GetComponent<CardManager>();
        _animations = new();
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

        /*
        Sequence allAnimations = DOTween.Sequence();

        foreach (CardAnimation animation in _animations)
        {
            allAnimations.Join(animation.GetAnimationSequence());
        }

        await allAnimations.Play().OnComplete(() => { }).AsyncWaitForCompletion();
        */

        foreach (CardAnimation animation in _animations)
        {
            _ = animation.AnimateCards(activeNode, previousActiveNode);
        }

        await DOTween.Sequence().AppendInterval(3).OnComplete(() => { }).AsyncWaitForCompletion();

        FinishAnimation();

        // Auf eine Animation wird immer statisch nochmal alles gesetzt
        SetCardsStatic(activeNode);
    }

    public void SetCardsStatic(CardNode activeNode)
    {
        PrepareStatic();

        foreach (CardAnimation animation in _animations)
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

        _cardMover.SetMainCardsRelativeToParent();

        _cardMover.SetHeightAndRotationOfInventory();

        _cardMover.SetInventoryCardsRelativeToParent();

        _cardInput.SetColliders();

        ClearAnimations();
    }

    public void AddAnimation(CardAnimation animation)
    {
        _animations.Add(animation);
    }

    public void ClearAnimations()
    {
        _animations.Clear();
    }

}
