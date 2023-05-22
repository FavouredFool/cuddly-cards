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


    public void Awake()
    {
        _cardManager = GetComponent<CardManager>();
    }

    public void ResetPosition(CardNode rootNode)
    {
        rootNode.TraverseContext(delegate (CardNode node)
        {
            node.Body.transform.position = Vector3.zero;
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

    public void MoveCardsForLayoutStatic(CardNode mainNode, CardNode rootNode)
    {
        MoveCard(mainNode, _playSpaceBottomLeft);

        if (mainNode != rootNode)
        {
            MoveCard(mainNode.Parent, new Vector2(_playSpaceBottomLeft.x, _playSpaceTopRight.y));

            if (mainNode.Parent != rootNode)
            {
                MoveCard(rootNode, _playSpaceTopRight);
            }
        }

        for (int i = 0; i < mainNode.Children.Count; i++)
        {
            MoveCard(mainNode.Children[i], new Vector2(i * _childrenDistance - _childrenStartOffset, _playSpaceBottomLeft.y));
        }
    }

    public void MoveCardsForLayoutAnimated(CardNode mainNode, CardNode oldMainNode, CardNode rootNode)
    {
        // we dont know what the new main node is relative to the old main node. Could be a child, could be back, could be root.

        float timeTotal = 0;

        if (oldMainNode.Children.Contains(mainNode))
        {
            ChildClickedAnimated(mainNode, oldMainNode, oldMainNode.Parent, rootNode, mainNode.Children, oldMainNode.Children);
            timeTotal = _verticalTime * 2 + _horizontalTime * 2 + _waitTime;
        }
        else if (oldMainNode.Parent == mainNode)
        {
            BackClickedAnimated();
            timeTotal = _verticalTime * 2 + _horizontalTime * 2 + _waitTime;
        }

        // MAKE SURE THAT THE TIMER IS A BIT LONGER THAN THE MAXIMUM TWEENING TIME
        Sequence timerSequence = DOTween.Sequence()
            .AppendInterval(timeTotal + 0.01f)
            .OnComplete(() => { _cardManager.FinishLayout(); });
    }

    public void BackClickedAnimated()
    {
        return;
        /*

        // NEW CHILDREN
        for (int i = _newChildNodes.Count - 1; i >= 0; i--)
        {
            CardNode newChild = _newChildNodes[i];

            Sequence childSequence = DOTween.Sequence();

            Transform childTransform = newChild.Body.transform;

            if (_oldMainNode == newChild)
            {
                Transform oldMainTransform = childTransform;

                int oldChildrenCount = 0;
                foreach (CardNode oldChild in _oldChildNodes)
                {
                    oldChildrenCount += oldChild.NodeCountBody();
                }

                int cardAmountOldMain = 1 + oldChildrenCount;
                // temporary for fixes
                //int cardAmountParents = _oldMainNode.NodeCountBodyRightSide();
                int cardAmountParents = 0;
                

                childSequence.Append(oldMainTransform.DOMoveY((cardAmountOldMain + cardAmountParents) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                    .AppendInterval(_horizontalTime + _waitTime)
                    .Append(oldMainTransform.DOMoveX(newChild.Parent.Children.IndexOf(newChild) * 1.125f - 1f, _horizontalTime).SetEase(_horizontalEasing))
                    .Append(childTransform.DOMoveY(cardAmountOldMain * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));
            }
            else
            {
                // other new Child stuff -> MMove down from back-position together with the top card (seperately)

                // they need to increase in Y too IF they are above the oldMain
                //childSequence.Append(childTransform.DoMove)
                childSequence.AppendInterval(_waitTime)
                .Append(childTransform.DOMoveX(newChild.Parent.Children.IndexOf(newChild) * 1.125f - 1f, _horizontalTime).SetEase(_horizontalEasing))
                .Append(childTransform.DOMoveY(newChild.NodeCountBody() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));

            }
        }
        /*
        cardAmount += _mainNode.NodeCountBody();
        mainCardsAmount = cardAmount;


        // NEW MAIN
        Transform mainTransform = _mainNode.Body.transform;

        Sequence mainSequence = DOTween.Sequence()
            .Append(mainTransform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .Append(mainTransform.DOMoveX(-2.5f, _horizontalTime).SetEase(_horizontalEasing))
            .AppendInterval(_waitTime + _horizontalTime)
            .Append(mainTransform.DOMoveY(CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));


        // OLD CHILDREN
        for (int i = _oldChildNodes.Count - 1; i >= 0; i--)
        {
            CardNode oldChild = _oldChildNodes[i];
            Transform oldChildTransform = oldChild.Body.transform;

            if (oldChild == _mainNode)
            {
                continue;
            }

            Sequence oldChildSequence = DOTween.Sequence()
                .Append(oldChildTransform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(oldChildTransform.DOMoveX(-2.5f, _horizontalTime).SetEase(_horizontalEasing))
                .OnComplete(() => { oldChild.Body.transform.parent = _oldMainNode.Body.transform; });

            cardAmount += oldChild.NodeCountBody();
        }


        if (_oldDiscardNode != null)
        {
            // OLD DISCARD
            Transform discardTransform = _oldDiscardNode.Body.transform;
            _oldBackNode.TraverseBodyUnparent();

            Sequence oldDiscardSequence = DOTween.Sequence()
                .Append(discardTransform.DOMoveY((_oldDiscardNode.NodeCountBody() + _oldBackNode.NodeCountBody()) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));

            // OLD BACK
            Transform oldBackTransform = _oldBackNode.Body.transform;
            int cardAmountOldBack = _oldBackNode.NodeCountBodyRightSide() + _oldBackNode.NodeCountBody();

            Sequence oldBackSequence = DOTween.Sequence()
                .Append(oldBackTransform.DOMoveY(cardAmountOldBack * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .AppendInterval(_horizontalTime + _waitTime)
                .Append(oldBackTransform.DOMoveX(2.875f, _horizontalTime).SetEase(_horizontalEasing));
        }
        else if (_oldBackNode != null)
        {
            // OLD BACK WITHOUT DISCARD
            Transform oldBackTransform = _oldBackNode.Body.transform;
            Sequence oldBackSequence = DOTween.Sequence()
                .AppendInterval(_verticalTime + _horizontalTime + _waitTime)
                .Append(oldBackTransform.DOMoveX(2.875f, _horizontalTime).SetEase(_horizontalEasing));
        }
        */
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

        int upwardsCount = 1;
        int remainingCount = 1;

        foreach (CardNode child in backToBe.Children)
        {
            upwardsCount += child.NodeCountBody();
            if (child != mainToBe) remainingCount += child.NodeCountBody();
        }

        Sequence oldMainSequence = DOTween.Sequence()
            .Append(oldMainTransform.DOMoveY(upwardsCount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(oldMainTransform.DOMoveZ(_playSpaceTopRight.y, _horizontalTime).SetEase(_horizontalEasing))
            .Append(oldMainTransform.DOMoveY(remainingCount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));


        if (discard != null)
        {
            // discard
            Transform discardTransform = discard.Body.transform;

            // TODO LOOK INTO THIS
            discardToBe.UnparentCardBodiesBelowCardInPile(discard);

            discardTransform.DOMoveY((discard.NodeCountBody() + discardToBe.NodeCountBody()) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing);

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