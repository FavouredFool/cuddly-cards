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
    CardInventory _cardInventory;
    StateManager _stateManager;

    List<CardAnimation> _activeAnimations;
    List<CardAnimation> _allAnimations;

    public AnimationManager(CardManager cardManager, List<CardAnimation> animations)
    {
        _cardManager = cardManager;
        _cardMover = cardManager.GetCardMover();
        _cardInput = cardManager.GetCardInput();
        _cardInventory = cardManager.GetCardInventory();

        _stateManager = cardManager.GetStateManager();

        _allAnimations = animations;
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

        CardNode baseNode = _cardManager.GetBaseNode();

        ResetEntireBoard(baseNode);

        FinishStatic();
    }


    public void ResetEntireBoard(CardNode baseNode)
    {
        // MainPile
        ResetMain(baseNode);

        // InventoryPile
        ResetInventory(baseNode);
    }

    public void ResetMain(CardNode baseNode)
    {
        // das schaut nur den obersten State an -> könnte Probleme mit Inventory oder CloseUp machen?

        switch (_stateManager.GetStates().Peek())
        {
            case CoverState:
                ResetCover(baseNode);
                break;
            case InventoryState:
                ResetClose(baseNode);
                break;
            default:
                ResetDefault(baseNode);
                break;
        }
    }

    public void ResetCover(CardNode baseNode)
    {
        _cardManager.AddToTopLevelMainPile(baseNode);
        _cardMover.MoveCard(baseNode, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x) * 0.5f, _cardMover.GetPlaySpaceBottomLeft().y));
    }

    public void ResetClose(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.GetRootNode();
        // move in deck -> move out inventory

        _cardManager.AddToTopLevelMainPile(baseNode);
        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }
    }

    public void ResetDefault(CardNode baseNode)
    {
        CardNode rootNode = _cardManager.GetRootNode();
        _cardManager.AddToTopLevelMainPile(baseNode);
        _cardMover.MoveCard(baseNode, _cardMover.GetPlaySpaceBottomLeft());

        if (baseNode != rootNode)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Parent);
            _cardMover.MoveCard(baseNode.Parent, new Vector2(_cardMover.GetPlaySpaceBottomLeft().x, _cardMover.GetPlaySpaceTopRight().y));

            if (baseNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevelMainPile(rootNode);
                _cardMover.MoveCard(rootNode, _cardMover.GetPlaySpaceTopRight());
            }
        }

        for (int i = 0; i < baseNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevelMainPile(baseNode.Children[i]);
            _cardMover.MoveCard(baseNode.Children[i], new Vector2(i * _cardMover.GetChildrenDistance() - _cardMover.GetChildrenStartOffset(), _cardMover.GetPlaySpaceBottomLeft().y));
        }
    }

    public void ResetInventory(CardNode baseNode)
    {
        // Parentmovement

        if (_stateManager.GetStates().Peek() is CoverState)
        {
            _cardMover.MoveCard(_cardInventory.GetInventoryNode(), new Vector2(_cardMover.GetPlaySpaceTopRight().x + (_cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x), _cardMover.GetPlaySpaceBottomLeft().y));
        }
        else
        {
            _cardMover.MoveCard(_cardInventory.GetInventoryNode(), new Vector2(_cardMover.GetPlaySpaceTopRight().x, _cardMover.GetPlaySpaceBottomLeft().y));
        }

        if (_stateManager.GetStates().Peek() is not InventoryState)
        {
            // kept in
            CardNode inventoryNode = _cardInventory.GetInventoryNode();

            // Set no cardnodes toplevel
            inventoryNode[0].IsTopLevel = false;
            foreach (CardNode node in inventoryNode[0].Children)
            {
                node.IsTopLevel = false;
            }
            inventoryNode[1].IsTopLevel = false;
            foreach (CardNode node in inventoryNode[1].Children)
            {
                node.IsTopLevel = false;
            }
        }
        else
        {
            // fanned out
            CardNode inventoryNode = _cardInventory.GetInventoryNode();


            // Set all cardnodes toplevel
            inventoryNode[0].IsTopLevel = true;
            foreach (CardNode node in inventoryNode[0].Children)
            {
                node.IsTopLevel = true;
            }
            inventoryNode[1].IsTopLevel = true;
            foreach (CardNode node in inventoryNode[1].Children)
            {
                node.IsTopLevel = true;
            }

            float totalSpace = _cardMover.GetPlaySpaceTopRight().x - _cardMover.GetPlaySpaceBottomLeft().x;
            float fannedCardSpace = (totalSpace - 3 * _cardMover.GetBorder()) * 0.5f;

            float dialogueOffset = _cardMover.GetPlaySpaceBottomLeft().x + 2 * _cardMover.GetBorder() + fannedCardSpace;
            FanCardsFromInventorySubcardStatic(inventoryNode[0], dialogueOffset, fannedCardSpace);

            float keyOffset = _cardMover.GetPlaySpaceBottomLeft().x + _cardMover.GetBorder();
            FanCardsFromInventorySubcardStatic(inventoryNode[1], keyOffset, fannedCardSpace);


        }
    }

    public void FanCardsFromInventorySubcardStatic(CardNode inventorySubcard, float startFanX, float fannedCardSpace)
    {
        int totalChildCards = inventorySubcard.Children.Count;

        _cardMover.MoveCard(inventorySubcard, new Vector2(startFanX + fannedCardSpace, _cardMover.GetPlaySpaceBottomLeft().y));

        float cardPercentage = fannedCardSpace / (CardInfo.CARDWIDTH * totalChildCards);

        for (int i = 0; i < totalChildCards; i++)
        {
            _cardMover.MoveCard(inventorySubcard[totalChildCards - 1 - i], new Vector2(startFanX + i * CardInfo.CARDWIDTH * cardPercentage, _cardMover.GetPlaySpaceBottomLeft().y));
        }
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
