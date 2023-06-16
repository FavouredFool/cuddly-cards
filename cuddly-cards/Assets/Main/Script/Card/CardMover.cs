using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    [Header("CardPositions")]
    [SerializeField]
    Vector2 _playSpaceBottomLeft = new Vector2(-2.5f, 0);

    [SerializeField]
    Vector2 _playSpaceTopRight = new Vector2(2.875f, 2.5f);

    [SerializeField, Range(0, 2f)]
    float _childrenDistance = 1.125f;

    [SerializeField, Range(0, 2f)]
    float _childrenStartOffset = 1;


    [Header("CardMovement")]
    [SerializeField, Range(0f, 1)]
    float _verticalTime = 0.5f;

    [SerializeField, Range(0.1f, 210)]
    float _horizontalTime = 1f;

    [SerializeField, Range(0f, 210)]
    float _waitTime = 1f;

    [SerializeField]
    Ease _horizontalEasing;

    [SerializeField]
    Ease _verticalEasing;

    bool _isAnimating = false;

    public bool IsAnimatingFlag { get { return _isAnimating; } set { _isAnimating = value; } }

    CardManager _cardManager;

    public void Awake()
    {
        _cardManager = GetComponent<CardManager>();
    }

    public void LateUpdate()
    {
        // Move Cards with custom parenting structure per frame
        if (!IsAnimatingFlag)
        {
            return;
        }

        SetCardsRelativeToParent();
    }

    public void SetCardsRelativeToParent()
    {
        List<CardNode> topLevelNodes = _cardManager.GetTopLevelNodes();
        foreach (CardNode topLevel in topLevelNodes)
        {
            int size = 1;

            foreach (CardNode childNode in topLevel.Children)
            {
                size += childNode.SetPositionsRecursive(size);
            }
        }
    }

    public void ResetPosition(CardNode rootNode)
    {
        rootNode.TraverseContext(delegate (CardNode node)
        {
            node.Body.transform.position = Vector3.zero;
            return true;
        });
    }

    public void RemoveParenting(CardNode rootNode)
    {
        rootNode.TraverseContext(delegate (CardNode node)
        {
            node.Body.transform.parent = _cardFolder;
            return true;
        });
    }

    public void MoveCard(CardNode card, Vector2 position)
    {
        card.Body.transform.localPosition = new Vector3(position.x, card.Body.transform.localPosition.y, position.y);
    }

    public void SetHeightOfTopLevelNodes()
    {
        foreach (CardNode node in _cardManager.GetTopLevelNodes())
        {
            node.Body.SetHeight(node.NodeCountBody());
        }
    }

    public void MoveCardsForStartLayoutStatic(CardNode rootNode)
    {
        MoveCard(rootNode, new Vector2(_playSpaceBottomLeft.x  + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f, _playSpaceBottomLeft.y));
    }

    public void MoveCardsForLayoutStatic(CardNode pressedNode, CardNode rootNode)
    {
        _cardManager.AddToTopLevel(pressedNode);
        MoveCard(pressedNode, _playSpaceBottomLeft);

        if (pressedNode != rootNode)
        {
            _cardManager.AddToTopLevel(pressedNode.Parent);
            MoveCard(pressedNode.Parent, new Vector2(_playSpaceBottomLeft.x, _playSpaceTopRight.y));

            if (pressedNode.Parent != rootNode)
            {
                _cardManager.AddToTopLevel(rootNode);
                MoveCard(rootNode, _playSpaceTopRight);
            }
        }

        for (int i = 0; i < pressedNode.Children.Count; i++)
        {
            _cardManager.AddToTopLevel(pressedNode.Children[i]);
            MoveCard(pressedNode.Children[i], new Vector2(i * _childrenDistance - _childrenStartOffset, _playSpaceBottomLeft.y));
        }
    }

    public void MoveCardsForLayoutAnimated(CardNode mainToBe, CardNode previousMain, CardNode rootNode, bool isStartLayout, bool activateStartLayout)
    {
        float timeTotal = 0;
        IsAnimatingFlag = true;

        if (activateStartLayout)
        {
            StartLayoutEnteredAnimated(rootNode, previousMain);
            timeTotal = _verticalTime * 2 + _horizontalTime * 3 + 2 * _waitTime;
        }
        else
        {
            if (isStartLayout)
            {
                StartLayoutExitedAnimated(rootNode);
                timeTotal = _verticalTime * 2 + _horizontalTime * 2 + _waitTime;
            }
            else if (previousMain.Children.Contains(mainToBe))
            {
                ChildClickedAnimated(mainToBe, previousMain, previousMain.Parent, rootNode, mainToBe.Children, previousMain.Children);
                timeTotal = _verticalTime * 2 + _horizontalTime * 2 + _waitTime;
            }
            else if (previousMain.Parent == mainToBe)
            {
                BackClickedAnimated(mainToBe, mainToBe.Parent, previousMain, rootNode, previousMain.Children, mainToBe.Children);
                timeTotal = _verticalTime * 2 + _horizontalTime * 2 + _waitTime;
            }
        }

        // MAKE SURE THAT THE TIMER IS A BIT LONGER THAN THE MAXIMUM TWEENING TIME
        Sequence timerSequence = DOTween.Sequence()
            .AppendInterval(timeTotal + 0.01f)
            .OnComplete(() => { IsAnimatingFlag = false; _cardManager.FinishLayout(activateStartLayout); });
    }

    public void StartLayoutEnteredAnimated(CardNode rootNode, CardNode mainNode)
    {

        // -------------- CHILDREN ---------------------

        List<CardNode> children = mainNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];
            _cardManager.AddToTopLevel(child);

            DOTween.Sequence()
                .Append(TweenY(child, child.NodeCountUpToCardInPile(rootNode)))
                .Append(TweenX(child, _playSpaceBottomLeft.x))
                .AppendInterval(2 * _waitTime + _horizontalTime)
                .Append(TweenX(child, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f));
        }

        // -------------- MAIN ---------------------

        _cardManager.AddToTopLevel(mainNode);
        DOTween.Sequence()
            .Append(TweenY(mainNode, mainNode.NodeCountUpToCardInPile(rootNode)))
            .AppendInterval(2 * _horizontalTime + 2 * _waitTime)
            .Append(TweenX(mainNode, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f));

        // -------------- BACK ---------------------

        CardNode backNode = mainNode.Parent;
        _cardManager.AddToTopLevel(backNode);

        List<CardNode> lowerTopMostCardsBack = mainNode.GetTopMostCardBodiesBelowCardInPile(backNode);

        foreach (CardNode node in lowerTopMostCardsBack)
        {
            _cardManager.AddToTopLevel(node);
        }

        List<CardNode> animatingNodesBack = lowerTopMostCardsBack;
        animatingNodesBack.Add(backNode);

        foreach (CardNode node in animatingNodesBack)
        {
            DOTween.Sequence()
                .Append(TweenY(node, node.NodeCountUpToCardInPile(rootNode)))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(TweenZ(node, _playSpaceBottomLeft.y))
                .AppendInterval(_waitTime)
                .Append(TweenX(node, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f));
        }


        // -------------- ROOT ---------------------

        _cardManager.AddToTopLevel(rootNode);
        List<CardNode> lowerTopMostCardsRoot = backNode.GetTopMostCardBodiesBelowCardInPile(rootNode);

        foreach (CardNode node in lowerTopMostCardsRoot)
        {
            _cardManager.AddToTopLevel(node);
        }

        List<CardNode> animatingNodesRoot = lowerTopMostCardsRoot;
        animatingNodesRoot.Add(rootNode);

        foreach (CardNode node in animatingNodesRoot)
        {
            DOTween.Sequence()
                .Append(TweenY(node, node.NodeCountUpToCardInPile(rootNode)))
                .Append(TweenX(node, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(TweenZ(node, _playSpaceBottomLeft.y))
                .AppendInterval(_waitTime)
                .Append(TweenX(node, _playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f));
        }
    }

    public void StartLayoutExitedAnimated(CardNode rootNode)
    {
        _cardManager.AddToTopLevel(rootNode);
        DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(TweenX(rootNode, _playSpaceBottomLeft.x))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(TweenY(rootNode, 1));

        for (int i = 0; i < rootNode.Children.Count; i++)
        {
            CardNode childNode = rootNode.Children[i];
            _cardManager.AddToTopLevel(childNode);

            DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(TweenX(childNode, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(TweenX(childNode, i * _childrenDistance - _childrenStartOffset))
                .Append(TweenY(childNode, childNode.NodeCountContext()));
        }
    }

    public void BackClickedAnimated(CardNode mainToBe, CardNode backToBe, CardNode previousMain, CardNode rootNode, List<CardNode> previousChilds, List<CardNode> childsToBe)
    {
        CardNode discard = backToBe != null && backToBe != rootNode ? rootNode : null;
        
        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            _cardManager.AddToTopLevel(newChild);

            Sequence childSequence = DOTween.Sequence();

            if (previousMain == newChild)
            {
                // ------------- PREVIOUS MAIN ----------------
                
                childSequence
                    .Append(TweenY(previousMain, previousMain.NodeCountUpToCardInPile(mainToBe)))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(TweenX(previousMain, newChild.Parent.Children.IndexOf(newChild) * _childrenDistance - _childrenStartOffset))
                    .Append(TweenY(previousMain, previousMain.NodeCountUpToCardInPile(previousMain)));

                // ------------- PREVIOUS CHILDREN ----------------
                for (int j = previousChilds.Count - 1; j >= 0; j--)
                {
                    CardNode oldChild = previousChilds[j];

                    _cardManager.AddToTopLevel(oldChild);

                    DOTween.Sequence()
                        .Append(TweenY(oldChild, oldChild.NodeCountUpToCardInPile(mainToBe)))
                        .Append(TweenX(oldChild, _playSpaceBottomLeft.x))
                        .AppendInterval(_waitTime)
                        .Append(TweenX(oldChild, newChild.Parent.Children.IndexOf(newChild) * _childrenDistance - _childrenStartOffset))
                        .Append(TweenY(oldChild, oldChild.NodeCountUpToCardInPile(previousMain)));
                }

            }
            else
            {
                // ------------- NEW CHILDREN ----------------

                childSequence
                    .Append(TweenY(newChild, newChild.NodeCountUpToCardInPile(mainToBe)))
                    .Append(TweenZ(newChild, _playSpaceBottomLeft.y))
                    .AppendInterval(_waitTime)
                    .Append(TweenX(newChild, newChild.Parent.Children.IndexOf(newChild) * _childrenDistance - _childrenStartOffset))
                    .Append(TweenY(newChild, newChild.NodeCountContext()));
            }
        }

        // ------------- NEW MAIN ----------------

        _cardManager.AddToTopLevel(mainToBe);
        DOTween.Sequence()
            .Append(TweenY(mainToBe, mainToBe.NodeCountContext() + mainToBe.NodeCountBelowCardBodyInPile(rootNode)))
            .Append(TweenZ(mainToBe, _playSpaceBottomLeft.y))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(TweenY(mainToBe, 1));


        if (discard != null)
        {
            _cardManager.AddToTopLevel(discard);
            _cardManager.AddToTopLevel(backToBe);

            // ------------- DISCARD ----------------

            int discardHeight = discard.NodeCountBody();

            List<CardNode> lowerTopMostCardsRoot = backToBe.GetTopMostCardBodiesBelowCardInPile(rootNode);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevel(node);
            }

            DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime + _horizontalTime)
                .Append(TweenY(discard, discardHeight));

            // ------------- BackToBe ----------------


            DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(TweenX(backToBe, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(TweenY(backToBe, backToBe.NodeCountBody()));
        }
        else if (backToBe != null)
        {
            // ------------- BackToBe ----------------

            _cardManager.AddToTopLevel(backToBe);

            DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(TweenX(backToBe, _playSpaceBottomLeft.x));
        }
    }

    public void ChildClickedAnimated(CardNode mainToBe, CardNode backToBe, CardNode discardToBe, CardNode rootNode, List<CardNode> childsToBe, List<CardNode> previousChilds)
    {
        CardNode discard = discardToBe != null && discardToBe != rootNode ? rootNode : null;

        // ------------- CHILDS TO BE ----------------

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            _cardManager.AddToTopLevel(newChild);

            DOTween.Sequence()
                .Append(TweenY(newChild, newChild.NodeCountUpToCardInPile(backToBe)))
                .Append(TweenX(newChild, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(TweenX(newChild, newChild.Parent.Children.IndexOf(newChild) * _childrenDistance - _childrenStartOffset))
                .Append(TweenY(newChild, newChild.NodeCountContext()));
        }


        // ------------- MAIN TO BE ----------------

        _cardManager.AddToTopLevel(mainToBe);
        DOTween.Sequence()
            .Append(TweenY(mainToBe, mainToBe.NodeCountUpToCardInPile(backToBe)))
            .Append(TweenX(mainToBe, _playSpaceBottomLeft.x))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(TweenY(mainToBe, 1));


        // ------------- Previous Children ----------------

        int height = 0;

        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = previousChilds[i];

            if (previousChild == mainToBe)
            {
                continue;
            }

            _cardManager.AddToTopLevel(previousChild);

            height += previousChild.NodeCountContext();

            DOTween.Sequence()
                .Append(TweenY(previousChild, previousChild.NodeCountUpToCardInPile(backToBe)))
                .Append(TweenX(previousChild, _playSpaceBottomLeft.x))
                .AppendInterval(_waitTime)
                .Append(TweenZ(previousChild, _playSpaceTopRight.y))
                .Append(TweenY(previousChild, height));

        }

        // ------------- BackToBe ----------------

        _cardManager.AddToTopLevel(backToBe);
        DOTween.Sequence()
            .Append(TweenY(backToBe, backToBe.NodeCountContext()))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(TweenZ(backToBe, _playSpaceTopRight.y))
            .Append(TweenY(backToBe, backToBe.NodeCountContext() - mainToBe.NodeCountContext()));
        

        // ------------- Discard & DiscardToBe ----------------

        if (discard != null)
        {
            _cardManager.AddToTopLevel(discard);
            _cardManager.AddToTopLevel(discardToBe);

            // height needs to be calculated before the deck is split in two, because otherwise new top-levels would be overlooked (this is a bit ugly)
            int discardHeight = discard.NodeCountBody() + discardToBe.NodeCountBody();
            int discardToBeHeight = discardToBe.NodeCountUpToCardInPileCardBodySensitive(rootNode);

            List<CardNode> lowerTopMostCardsRoot = discardToBe.GetTopMostCardBodiesBelowCardInPile(rootNode);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevel(node);
            }

            TweenY(rootNode, discardHeight);

            DOTween.Sequence()
                .Append(TweenY(discardToBe, discardToBeHeight))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(TweenX(discardToBe, _playSpaceTopRight.x));
        }
        else if (discardToBe != null)
        {
            _cardManager.AddToTopLevel(discardToBe);
            DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(TweenX(discardToBe, _playSpaceTopRight.x));
        }
    }

    private Tween TweenX(CardNode main, float posX)
    {
        return main.Body.transform.DOMoveX(posX, _horizontalTime).SetEase(_horizontalEasing);
    }

    private Tween TweenY(CardNode main, int height)
    {
        return main.Body.transform.DOMoveY(height * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing);
    }

    private Tween TweenZ(CardNode main, float posZ)
    {
        return main.Body.transform.DOMoveZ(posZ, _horizontalTime).SetEase(_horizontalEasing);
    }
}