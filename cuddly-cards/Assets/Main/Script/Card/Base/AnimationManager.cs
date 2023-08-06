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
        await PlayAnimations(activeNode, _cardManager.BaseNode);
    }
    public async Task PlayAnimations(CardNode activeNode, CardNode baseNode)
    {
        await SetCardsAnimated(activeNode, baseNode);
    }

    public async Task SetCardsAnimated(CardNode activeNode, CardNode baseNode)
    {
        PrepareAnimation(activeNode, baseNode);

        Sequence allAnimations = DOTween.Sequence();

        foreach (CardAnimation animation in _activeAnimations)
        {
            Sequence animSequence = animation.GetAnimationSequence(activeNode, baseNode);
            allAnimations.Join(animSequence);
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

    public void PrepareAnimation(CardNode activeNode, CardNode baseNode)
    {
        _cardInput.RemoveColliders();

        _cardManager.ClearTopLevelNodesMainPile();

        SetTopLevelNodes(activeNode, baseNode);

        _cardMover.IsAnimatingFlag = true;
    }

    public void SetTopLevelNodes(CardNode activeNode, CardNode baseNode)
    {
        CardNode rootNode = _cardManager.RootNode;
        _cardManager.AddToTopLevel(_cardManager.CardInventory.InventoryNode);

        if (activeNode != baseNode.Parent)
        {
            _cardManager.AddToTopLevel(baseNode);
        }
        
        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevel(baseNode.Children[i]);
        }

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevel(baseNode.Parent);

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevel(rootNode);
            }
        }
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

        _cardMover.ResetPositionAndRotation(_cardManager.RootNode, _cardManager.CardInventory.InventoryNode);
    }

    public void FinishStatic()
    {
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
