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

        foreach (CardNode topLevel in _cardManager.GetTopLevelNodes())
        {
            foreach (CardNode childNode in topLevel.Children)
            {
                childNode.SetPositionsRecursive(1);
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

    public void MoveCardsForStartLayoutAnimated(CardNode rootNode, CardNode oldMain)
    {
        // Children

        List<CardNode> previousChilds = oldMain.Children;

        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = previousChilds[i];
            Transform oldChildTransform = previousChild.Body.transform;

            Sequence oldChildSequence = DOTween.Sequence()
                .Append(oldChildTransform.DOMoveY(previousChild.NodeCountUpToCardInPile(rootNode) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(oldChildTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
                .OnComplete(() => {
                    previousChild.Body.transform.parent = oldMain.Body.transform;
                });
        }

        // old Main
        Transform oldMainTransform = oldMain.Body.transform;

        int heightAmount = oldMain.NodeCountContext() + oldMain.NodeCountBelowCardBodyInPile(rootNode);

        Sequence oldMainSequence = DOTween.Sequence()
            .Append(oldMainTransform.DOMoveY(heightAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .AppendInterval(2 * _horizontalTime + 2 * _waitTime)
            .OnComplete(() => { oldMainTransform.parent = oldMain.Parent.Body.transform; });

        // old parent

        Transform oldParentTransform = oldMain.Parent.Body.transform;
        List<CardNode> unparentedCards = oldMain.UnparentCardBodiesBelowCardInPile(oldMain.Parent);
        // I need all unparented elements here (can be more than one, are right next to each other probably)
        // then i can move them seperately by NodeCountCardBodyInPile(rootNode)

        foreach (CardNode card in unparentedCards)
        {
            card.Body.transform.DOMoveY(card.NodeCountUpToCardInPile(rootNode) * CardInfo.CARDHEIGHT, _verticalTime);
        }


        oldParentTransform.DOMoveY((oldMain.Parent.NodeCountContext() + oldMain.Parent.NodeCountBelowCardBodyInPile(rootNode)) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing)
            .OnComplete(() =>
            {
                oldMain.ReparentCardBodiesBelowCardInPile(oldMain.Parent);
                oldParentTransform.DOMoveX(oldParentTransform.position.x, _horizontalTime)
                .OnComplete(() => { oldParentTransform.parent = rootNode.Body.transform;
                }); });


        // rootNode

        Transform rootNodeTransform = rootNode.Body.transform;

        oldMain.Parent.UnparentCardBodiesBelowCardInPile(rootNode);


        //Sequence rootNodeSequence = DOTween.Sequence()
        //.Append(rootNodeTransform.DOMoveY(rootNode.NodeCountBody(), _verticalTime).SetEase(_verticalEasing)).OnComplete>
        //.Append(rootNodeTransform.DoMoveX())

        rootNodeTransform.DOMoveY(rootNode.NodeCountContext() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing)
            .OnComplete(() =>
            {
                oldMain.Parent.ReparentCardBodiesBelowCardInPile(rootNode);
                rootNodeTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime)
                .OnComplete(()=> { rootNodeTransform.DOMoveX(rootNodeTransform.position.x, _waitTime)
                     .OnComplete(() => { rootNodeTransform.DOMoveZ(_playSpaceBottomLeft.y, _horizontalTime)
                          .OnComplete(() => { rootNodeTransform.DOMoveX(rootNodeTransform.position.x, _waitTime)
                              .OnComplete(() => { rootNodeTransform.DOMoveX(_playSpaceBottomLeft.x + (_playSpaceTopRight.x - _playSpaceBottomLeft.x) * 0.5f, _horizontalTime);
                                  }); }); }); }); });


        Sequence timerSequence = DOTween.Sequence()
            .AppendInterval(_verticalTime * 2 + _horizontalTime * 3 +  2 * _waitTime + 0.01f)
            .OnComplete(() => { _cardManager.FinishStartLayout(); });
    }

    public void StartLayoutExitedAnimated(CardNode rootNode)
    {
        Transform rootTransform = rootNode.Body.transform;

        Sequence rootSequence = DOTween.Sequence()
            .AppendInterval(_verticalTime)
            .Append(rootTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(rootTransform.DOMoveY(CardInfo.CARDHEIGHT, _verticalTime));

        for (int i = 0; i < rootNode.Children.Count; i++)
        {
            Transform childTransform = rootNode.Children[i].Body.transform;
            childTransform.parent = null;

            Sequence childSequence = DOTween.Sequence()
                .AppendInterval(_verticalTime)
                .Append(childTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime))
                .AppendInterval(_waitTime)
                .Append(childTransform.DOMoveX(i * _childrenDistance - _childrenStartOffset, _horizontalTime))
                .Append(childTransform.DOMoveY(rootNode.Children[i].NodeCountBody() * CardInfo.CARDHEIGHT, _verticalTime));
        }
    }

    public void MoveCardsForLayoutAnimated(CardNode mainToBe, CardNode previousMain, CardNode rootNode, bool isStartLayout)
    {
        _isAnimating = true;

        // we dont know what the new main node is relative to the old main node. Could be a child, could be back, could be root.

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

        // Childs to be
        for (int i = childsToBe.Count - 1; i >= 0; i--)
        {
            CardNode newChild = childsToBe[i];
            Transform childTransform = newChild.Body.transform;
            childTransform.parent = null;

            Sequence childSequence = DOTween.Sequence()
                .Append(childTransform.DOMoveY(newChild.NodeCountUpToCardInPile(backToBe) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(childTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(childTransform.DOMoveX(newChild.Parent.Children.IndexOf(newChild) * _childrenDistance - _childrenStartOffset, _horizontalTime).SetEase(_horizontalEasing))
                .Append(childTransform.DOMoveY(newChild.NodeCountBody() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));
        }

        // Main to be
        Transform mainTransform = mainToBe.Body.transform;
        Sequence mainSequence = DOTween.Sequence()
            .Append(mainTransform.DOMoveY(mainToBe.NodeCountUpToCardInPile(backToBe) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .Append(mainTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(mainTransform.DOMoveY(CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));


        // previous childs
        for (int i = previousChilds.Count - 1; i >= 0; i--)
        {
            CardNode previousChild = previousChilds[i];
            Transform oldChildTransform = previousChild.Body.transform;

            if (previousChild == mainToBe)
            {
                continue;
            }

            _cardManager.AddToTopLevel(previousChild);

            Sequence oldChildSequence = DOTween.Sequence()
                .Append(oldChildTransform.DOMoveY(previousChild.NodeCountUpToCardInPile(backToBe) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(oldChildTransform.DOMoveX(_playSpaceBottomLeft.x, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(oldChildTransform.DOMoveZ(_playSpaceTopRight.y, _horizontalTime).SetEase(_horizontalEasing))
                .OnComplete(() => {
                    if (previousChilds.IndexOf(previousChild) < previousChilds.IndexOf(mainToBe))
                    {
                        previousChild.Body.transform.parent = backToBe.Body.transform;
                    }
                });
        }

        // back to be
        Transform oldMainTransform = backToBe.Body.transform;

        Sequence oldMainSequence = DOTween.Sequence()
            .Append(oldMainTransform.DOMoveY(backToBe.NodeCountContext() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(oldMainTransform.DOMoveZ(_playSpaceTopRight.y, _horizontalTime).SetEase(_horizontalEasing))
            .Append(oldMainTransform.DOMoveY((backToBe.NodeCountContext() - mainToBe.NodeCountContext()) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));


        if (discard != null)
        {
            // discard
            Transform discardTransform = discard.Body.transform;

            _cardManager.AddToTopLevel(discardToBe);

            // TODO LOOK INTO THIS
            discardToBe.MakeTopCardBodiesBelowCardInPile(discard, _cardManager);

            discardTransform.DOMoveY(discard.NodeCountContext() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing);

            // discard to be
            Transform oldBackTransform = discardToBe.Body.transform;

            Sequence oldBackSequence = DOTween.Sequence()
                .Append(oldBackTransform.DOMoveY(discardToBe.NodeCountUpToCardInPile(discard) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(oldBackTransform.DOMoveX(_playSpaceTopRight.x, _horizontalTime).SetEase(_horizontalEasing));
        }
        else if (discardToBe != null)
        {
            // discard to be without discord
            Transform oldBackTransform = discardToBe.Body.transform;
            Sequence oldBackSequence = DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(oldBackTransform.DOMoveX(_playSpaceTopRight.x, _horizontalTime).SetEase(_horizontalEasing));
        }

    }
}