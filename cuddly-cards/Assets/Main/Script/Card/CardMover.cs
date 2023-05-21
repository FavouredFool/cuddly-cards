using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

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

    CardNode _mainNode;
    CardNode _newBackNode;
    CardNode _newDiscardNode;
    List<CardNode> _newChildNodes;

    CardNode _oldMainNode;
    CardNode _oldBackNode;
    CardNode _oldDiscardNode;
    List<CardNode> _oldChildNodes;



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
                cardNode.Body.transform.parent ??= _cardFolder;

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
        MoveCard(mainNode, new Vector2(-2.5f, 0));

        if (mainNode != rootNode)
        {
            MoveCard(mainNode.Parent, new Vector2(-2.5f, 2.5f));

            if (mainNode.Parent != rootNode)
            {
                MoveCard(rootNode, new Vector2(2.875f, 2.5f));
            }
        }

        for (int i = 0; i < mainNode.Children.Count; i++)
        {
            MoveCard(mainNode.Children[i], new Vector2(i * 1.125f - 1f, 0));
        }
    }

    public void MoveCardsForLayoutAnimated(CardNode activeNode, CardNode oldActiveNode, CardNode rootNode)
    {
        _mainNode = activeNode;
        _newBackNode = activeNode.Parent;
        _newDiscardNode = _newBackNode != rootNode && _newBackNode != null ? rootNode : null;
        _newChildNodes = activeNode.Children;

        _oldMainNode = oldActiveNode;
        _oldBackNode = oldActiveNode.Parent;
        _oldDiscardNode = _oldBackNode != rootNode && _oldBackNode != null ? rootNode : null;
        _oldChildNodes = oldActiveNode.Children;

        if (_oldChildNodes.Contains(_mainNode))
        {
            ChildClickedAnimated();
        }
        else if (_oldBackNode == _mainNode)
        {
            BackClickedAnimated();
        }

        // MAKE SURE THAT THE TIMER IS A BIT LONGER THAN THE MAXIMUM TWEENING TIME
        Sequence timerSequence = DOTween.Sequence()
            .AppendInterval(_horizontalTime * 2 + _verticalTime * 2 + _waitTime + 0.01f)
            .OnComplete(() => { _cardManager.FinishLayout(); });
    }

    public void BackClickedAnimated()
    {
        return;

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

    public int CardAmountBelowNodeThroughList(CardNode activeNode, CardNode topOfPile)
    {
        List<CardNode> cardList = new();

        // Turn pile into a list
        topOfPile.TraverseContext(delegate (CardNode node)
        {
            cardList.Add(node);
            return true;
        });

        cardList.Reverse();
        return cardList.IndexOf(activeNode)+1;
    }

    public int CardAmountBelowNodeInPile(CardNode activeNode, CardNode topOfPile)
    {
        // The amount of cards that are below the cardbody that is provided inside of the cardstack that is provided.
        // I cant believe this works

        int cardAmount = 0;

        for (int i = topOfPile.Children.Count - 1; i >= 0; i--)
        {
            if (activeNode == topOfPile.Children[i])
            {
                return -cardAmount;
            }

            int result = CardAmountBelowNodeInPile(activeNode, topOfPile.Children[i]);

            if (result >= 0)
            {
                return -cardAmount + result;
            }

            cardAmount += result;
        }

        // Important! Add this at the END
        cardAmount -= 1;

        return cardAmount;
    }


    public void ChildClickedAnimated()
    {
        // NEW CHILDREN
        for (int i = _newChildNodes.Count - 1; i >= 0; i--)
        {
            CardNode newChild = _newChildNodes[i];
            Transform childTransform = newChild.Body.transform;
            childTransform.parent = null;

            // Three Versions of doing the exact same thing. Could the List be kept in memory?

            int cardHeight = newChild.NodeCountBodyRightSide(_oldMainNode) + newChild.NodeCountBody();

            Sequence childSequence = DOTween.Sequence()
                .Append(childTransform.DOMoveY(cardHeight * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(childTransform.DOMoveX(-2.5f, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(childTransform.DOMoveX(newChild.Parent.Children.IndexOf(newChild) * 1.125f - 1f, _horizontalTime).SetEase(_horizontalEasing))
                .Append(childTransform.DOMoveY(newChild.NodeCountBody() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));
        }



        // NEW MAIN

        int cardAmountMain = _mainNode.NodeCountBodyRightSide(_oldMainNode) + _mainNode.NodeCountBody();

        Transform mainTransform = _mainNode.Body.transform;
        Sequence mainSequence = DOTween.Sequence()
            .Append(mainTransform.DOMoveY(cardAmountMain * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
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
            Debug.Log("----");
            Debug.Log(CardAmountBelowNodeThroughList(oldChild, oldChild.Parent));
            Debug.Log(CardAmountBelowNodeInPile(oldChild, oldChild.Parent) + oldChild.NodeCountBody());
            Debug.Log(oldChild.NodeCountBodyRightSide(_oldMainNode) + oldChild.NodeCountBody());

            int cardAmountOldChild = oldChild.NodeCountBodyRightSide(_oldMainNode) + oldChild.NodeCountBody();

            Sequence oldChildSequence = DOTween.Sequence()
                .Append(oldChildTransform.DOMoveY(cardAmountOldChild * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(oldChildTransform.DOMoveX(-2.5f, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(oldChildTransform.DOMoveZ(2.5f, _horizontalTime).SetEase(_horizontalEasing))
                .OnComplete(() => {
                    if (_oldChildNodes.IndexOf(oldChild) < _oldChildNodes.IndexOf(_mainNode))
                    {
                        oldChild.Body.transform.parent = _oldMainNode.Body.transform;
                    }
                });
        }

        Transform oldMainTransform = _oldMainNode.Body.transform;

        int upwardsCount = 1;

        foreach (CardNode child in _oldMainNode.Children)
        {
            upwardsCount += child.NodeCountBody();
        }

        int remainingCount = 1;

        foreach (CardNode child in _oldMainNode.Children)
        {
            if (child == _mainNode)
            {
                continue;
            }

            remainingCount += child.NodeCountBody();
        }

        Sequence oldMainSequence = DOTween.Sequence()
            .Append(oldMainTransform.DOMoveY(upwardsCount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(oldMainTransform.DOMoveZ(2.5f, _horizontalTime).SetEase(_horizontalEasing))
            .Append(oldMainTransform.DOMoveY(remainingCount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));


        if (_oldDiscardNode != null)
        {
            // OLD DISCARD
            Transform discardTransform = _oldDiscardNode.Body.transform;
            _oldBackNode.TraverseBodyUnparent();

            Sequence oldDiscardSequence = DOTween.Sequence()
                .Append(discardTransform.DOMoveY((_oldDiscardNode.NodeCountBody() + _oldBackNode.NodeCountBody()) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));

            // OLD BACK
            Transform oldBackTransform = _oldBackNode.Body.transform;
            int cardAmountOldBack = _oldBackNode.NodeCountBodyRightSide(_oldDiscardNode) + _oldBackNode.NodeCountBody();

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

    }
}