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

    CardManager _cardManager;

    bool _isAnimating = false;


    public void Awake()
    {
        _cardManager = GetComponent<CardManager>();
    }

    public void LateUpdate()
    {
        // Move Cards with custom parenting structure per frame
        if (!_isAnimating)
        {
            return;
        }

        // Einmal durchgehen, alle setzen. Die Height wird runtergerechnet
        // simuliert parenting structure
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

    public void ParentCards(CardNode rootNode)
    {
        rootNode.TraverseContext(
            delegate (CardNode cardNode)
            {
                CardNode parent = cardNode.Parent;

                // If the node is top level, cut off any parenting
                if (cardNode.IsTopLevel) parent = null;

                cardNode.Body.transform.parent = parent?.Body.transform;
                cardNode.Body.transform.parent = cardNode.Body.transform.parent != null ? cardNode.Body.transform.parent : _cardFolder;

                return true;
            }
        );
    }

    public void PileFromParenting(List<CardNode> topLevelNodes)
    {
        foreach (CardNode node in topLevelNodes)
        {
            if (!node.IsTopLevel)
            {
                Debug.LogError("Tried to create pile from non-topLevel cardBody");
            }

            node.SetHeightRecursive(0);
            node.Body.SetHeight(node.NodeCountBody());
        }
    }

    public void MoveCardsForStartLayoutStatic(CardNode rootNode)
    {
        MoveCard(rootNode, new Vector2(_playSpaceBottomLeft.x  + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f, _playSpaceBottomLeft.y));
    }

    public void MoveCardsForLayoutStatic(CardNode pressedNode, CardNode previousNode, CardNode rootNode)
    {
        MoveCard(pressedNode, _playSpaceBottomLeft);

        if (pressedNode != rootNode)
        {
            MoveCard(pressedNode.Parent, new Vector2(_playSpaceBottomLeft.x, _playSpaceTopRight.y));

            if (pressedNode.Parent != rootNode)
            {
                MoveCard(rootNode, _playSpaceTopRight);
            }
        }

        for (int i = 0; i < pressedNode.Children.Count; i++)
        {
            MoveCard(pressedNode.Children[i], new Vector2(i * _childrenDistance - _childrenStartOffset, _playSpaceBottomLeft.y));
        }
    }

    public void MoveCardsForLayoutAnimated(CardNode mainToBe, CardNode previousMain, CardNode rootNode, bool isStartLayout)
    {
        _isAnimating = true;

        float timeTotal = 0;
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

        

        // MAKE SURE THAT THE TIMER IS A BIT LONGER THAN THE MAXIMUM TWEENING TIME
        Sequence timerSequence = DOTween.Sequence()
            .AppendInterval(timeTotal + 0.01f)
            .OnComplete(() => { _isAnimating = false; _cardManager.FinishLayout(); });
    }

    public void MoveCardsForStartLayoutAnimated(CardNode rootNode, CardNode mainNode)
    {
        _isAnimating = true;

        // -------------- CHILDREN ---------------------

        List<CardNode> children = mainNode.Children;

        for (int i = 0; i < children.Count; i++)
        {
            CardNode child = children[i];
            Transform childTransform = child.Body.transform;

            DOTween.Sequence()
                .Append(childTransform.DOMoveY(child.NodeCountUpToCardInPile(rootNode) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(childTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(2 * _waitTime + _horizontalTime)
                .Append(childTransform.DOMoveX(_playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f, _horizontalTime).SetEase(_horizontalEasing));
        }

        // -------------- MAIN ---------------------

        Transform mainTransform = mainNode.Body.transform;
        DOTween.Sequence()
            .Append(mainTransform.DOMoveY(mainNode.NodeCountUpToCardInPile(rootNode) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .AppendInterval(2 * _horizontalTime + 2 * _waitTime)
            .Append(mainTransform.DOMoveX(_playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f, _horizontalTime).SetEase(_horizontalEasing));

        // -------------- BACK ---------------------

        CardNode backNode = mainNode.Parent;

        List<CardNode> lowerTopMostCardsBack = mainNode.GetTopMostCardBodiesBelowCardInPile(backNode);

        foreach (CardNode node in lowerTopMostCardsBack)
        {
            _cardManager.AddToTopLevel(node);
        }

        List<CardNode> animatingNodesBack = lowerTopMostCardsBack;
        animatingNodesBack.Add(backNode);

        foreach (CardNode node in animatingNodesBack)
        {
            Transform nodeTransform = node.Body.transform;

            DOTween.Sequence()
                .Append(nodeTransform.DOMoveY(node.NodeCountUpToCardInPile(rootNode) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(nodeTransform.DOMoveZ(_playSpaceBottomLeft.y, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(nodeTransform.DOMoveX(_playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f, _horizontalTime).SetEase(_horizontalEasing));
        }
        

        // -------------- ROOT ---------------------

        List<CardNode> lowerTopMostCardsRoot = backNode.GetTopMostCardBodiesBelowCardInPile(rootNode);

        foreach (CardNode node in lowerTopMostCardsRoot)
        {
            _cardManager.AddToTopLevel(node);
        }

        List<CardNode> animatingNodesRoot = lowerTopMostCardsRoot;
        animatingNodesRoot.Add(rootNode);

        foreach (CardNode node in animatingNodesRoot)
        {
            Transform nodeTransform = node.Body.transform;

            DOTween.Sequence()
                .Append(nodeTransform.DOMoveY(node.NodeCountUpToCardInPile(rootNode) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(nodeTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(nodeTransform.DOMoveZ(_playSpaceBottomLeft.y, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(nodeTransform.DOMoveX(_playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f, _horizontalTime).SetEase(_horizontalEasing));
        }


        Sequence timerSequence = DOTween.Sequence()
            .AppendInterval(_verticalTime * 2 + _horizontalTime * 3 + 2 * _waitTime + 0.01f)
            .OnComplete(() => { _isAnimating = false; _cardManager.FinishStartLayout(); });
    }

    public void StartLayoutExitedAnimated(CardNode rootNode)
    {
        Transform rootTransform = rootNode.Body.transform;

        DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(rootTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(rootTransform.DOMoveY(CardInfo.CARDHEIGHT, _verticalTime));

        for (int i = 0; i < rootNode.Children.Count; i++)
        {
            CardNode childNode = rootNode.Children[i];
            Transform childTransform = childNode.Body.transform;

            DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(childTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime))
                .AppendInterval(_waitTime)
                .Append(childTransform.DOMoveX(i * _childrenDistance - _childrenStartOffset, _horizontalTime))
                .Append(childTransform.DOMoveY(childNode.NodeCountContext() * CardInfo.CARDHEIGHT, _verticalTime));
        }
    }

    public void BackClickedAnimated(CardNode mainToBe, CardNode backToBe, CardNode previousMain, CardNode rootNode, List<CardNode> previousChilds, List<CardNode> childsToBe)
    {
        CardNode discard = backToBe != null && backToBe != rootNode ? rootNode : null;

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];

            Sequence childSequence = DOTween.Sequence();

            Transform childTransform = newChild.Body.transform;

            if (previousMain == newChild)
            {
                Transform oldMainTransform = childTransform;                

                childSequence.Append(oldMainTransform.DOMoveY((previousMain.NodeCountContext() + previousMain.NodeCountBelowCardBodyInPile(rootNode)) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(oldMainTransform.DOMoveX(newChild.Parent.Children.IndexOf(newChild) * _childrenDistance - _childrenStartOffset, _horizontalTime).SetEase(_horizontalEasing))
                    .Append(childTransform.DOMoveY(previousMain.NodeCountContext() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));
            }
            else
            {
                // other new Child stuff -> MMove down from back-position together with the top card (seperately)

                // they need to increase in Y too IF they are above the oldMain

                int cardHeight = newChild.NodeCountBelowCardBodyInPile(rootNode) + newChild.NodeCountContext();

                childSequence.Append(childTransform.DOMoveY((cardHeight) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                    .Append(childTransform.DOMoveZ(_playSpaceBottomLeft.y, _horizontalTime).SetEase(_horizontalEasing))
                    .AppendInterval(_waitTime)
                    .Append(childTransform.DOMoveX(newChild.Parent.Children.IndexOf(newChild) * _childrenDistance - _childrenStartOffset, _horizontalTime).SetEase(_horizontalEasing))
                    .Append(childTransform.DOMoveY(newChild.NodeCountBody() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));
            }
        }

        // NEW MAIN
        Transform mainTransform = mainToBe.Body.transform;

        Sequence mainSequence = DOTween.Sequence()
            .Append(mainTransform.DOMoveY((mainToBe.NodeCountContext() + mainToBe.NodeCountBelowCardBodyInPile(rootNode)) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .Append(mainTransform.DOMoveZ(_playSpaceBottomLeft.y, _horizontalTime).SetEase(_horizontalEasing))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(mainTransform.DOMoveY(CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));



        // OLD CHILDREN
        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode oldChild = previousChilds[i];
            Transform oldChildTransform = oldChild.Body.transform;

            _cardManager.AddToTopLevel(oldChild);

            Sequence oldChildSequence = DOTween.Sequence()
                .Append(oldChildTransform.DOMoveY((oldChild.NodeCountContext() + oldChild.NodeCountBelowCardBodyInPile(mainToBe)) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(oldChildTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
                .OnComplete(() => { _cardManager.RemoveFromTopLevel(oldChild); });
        }

        if (discard != null)
        {
            _cardManager.AddToTopLevel(backToBe);

            // OLD DISCARD
            Transform discardTransform = discard.Body.transform;

            Sequence oldDiscardSequence = DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(discardTransform.DOMoveY((discard.NodeCountContext()) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));

            // OLD BACK
            Transform oldBackTransform = backToBe.Body.transform;

            Sequence oldBackSequence = DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(oldBackTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime + _horizontalTime)
                .Append(oldBackTransform.DOMoveY(backToBe.NodeCountBody() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));
        }
        else if (backToBe != null)
        {
            // OLD BACK WITHOUT DISCARD
            Transform oldBackTransform = backToBe.Body.transform;
            Sequence oldBackSequence = DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(oldBackTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing));
        }
    }

    public void ChildClickedAnimated(CardNode mainToBe, CardNode backToBe, CardNode discardToBe, CardNode rootNode, List<CardNode> childsToBe, List<CardNode> previousChilds)
    {
        CardNode discard = discardToBe != null && discardToBe != rootNode ? rootNode : null;


        // ------------- CHILDS TO BE ----------------

        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            Transform childTransform = newChild.Body.transform;

            DOTween.Sequence()
                .Append(childTransform.DOMoveY(newChild.NodeCountUpToCardInPile(backToBe) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(childTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(childTransform.DOMoveX(newChild.Parent.Children.IndexOf(newChild) * _childrenDistance - _childrenStartOffset, _horizontalTime).SetEase(_horizontalEasing))
                .Append(childTransform.DOMoveY(newChild.NodeCountContext() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));
        }

        
        // ------------- MAIN TO BE ----------------

        Transform mainTransform = mainToBe.Body.transform;

        DOTween.Sequence()
            .Append(mainTransform.DOMoveY(mainToBe.NodeCountUpToCardInPile(backToBe) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .Append(mainTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(mainTransform.DOMoveY(CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));


        // ------------- Previous Children ----------------

        int height = 0;

        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = previousChilds[i];
            Transform oldChildTransform = previousChild.Body.transform;

            if (previousChild == mainToBe)
            {
                continue;
            }

            _cardManager.AddToTopLevel(previousChild);

            height += previousChild.NodeCountContext();

            DOTween.Sequence()
            .Append(oldChildTransform.DOMoveY(previousChild.NodeCountUpToCardInPile(backToBe) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .Append(oldChildTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
            .AppendInterval(_waitTime)
            .Append(oldChildTransform.DOMoveZ(_playSpaceTopRight.y, _horizontalTime).SetEase(_horizontalEasing))
            .Append(oldChildTransform.DOMoveY(height * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));

        }

        // ------------- BackToBe ----------------

        Transform backToBeTransform = backToBe.Body.transform;

        DOTween.Sequence()
            .Append(backToBeTransform.DOMoveY(backToBe.NodeCountContext() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(backToBeTransform.DOMoveZ(_playSpaceTopRight.y, _horizontalTime).SetEase(_horizontalEasing))
            .Append(backToBeTransform.DOMoveY((backToBe.NodeCountContext() - mainToBe.NodeCountContext()) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));


        // ------------- Discard & DiscardToBe ----------------

        if (discard != null)
        {
            _cardManager.AddToTopLevel(discardToBe);

            // height needs to be calculated before the deck is split in two, because otherwise new top-levels would be overlooked (this is a bit ugly)
            int discardHeight = discard.NodeCountBody() + discardToBe.NodeCountBody();
            int discardToBeHeight = discardToBe.NodeCountUpToCardInPileCardBodySensitive(rootNode);

            List<CardNode> lowerTopMostCardsRoot = discardToBe.GetTopMostCardBodiesBelowCardInPile(rootNode);

            foreach (CardNode node in lowerTopMostCardsRoot)
            {
                _cardManager.AddToTopLevel(node);
            }

            rootNode.Body.transform.DOMoveY(discardHeight * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing);

            DOTween.Sequence()
                .Append(discardToBe.Body.transform.DOMoveY(discardToBeHeight * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(discardToBe.Body.transform.DOMoveX(_playSpaceTopRight.x, _horizontalTime).SetEase(_horizontalEasing));
        }
        else if (discardToBe != null)
        {
            DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(discardToBe.Body.transform.DOMoveX(_playSpaceTopRight.x, _horizontalTime).SetEase(_horizontalEasing));
        }



    }
}