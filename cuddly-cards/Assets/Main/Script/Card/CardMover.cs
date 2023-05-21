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

    [SerializeField, Range(0.1f, 2)]
    float _horizontalTime = 1f;

    [SerializeField, Range(0f, 2)]
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

        // MAKE SURE THAT THE TIMER IS A BIT LONGER THAN THE MAXIMUM TWEENING TIME
        Sequence timerSequence = DOTween.Sequence()
            .AppendInterval(_horizontalTime * 2 + _verticalTime * 2 + _waitTime + 0.01f)
            .OnComplete(() => { _cardManager.FinishLayout(); });
    }

    public void ChildClickedAnimated()
    {
        int cardAmount = 0;
        int mainCardsAmount;

        // NEW CHILDREN
        for (int i = _newChildNodes.Count - 1; i >= 0; i--)
        {
            CardNode newChild = _newChildNodes[i];
            Transform childTransform = newChild.Body.transform;

            cardAmount += newChild.NodeCountBody();

            Sequence childSequence = DOTween.Sequence()
                .Append(childTransform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
                .Append(childTransform.DOMoveX(-2.5f, _horizontalTime).SetEase(_horizontalEasing))
                .AppendInterval(_waitTime)
                .Append(childTransform.DOMoveX(newChild.Parent.Children.IndexOf(newChild) * 1.125f - 1f, _horizontalTime).SetEase(_horizontalEasing))
                .Append(childTransform.DOMoveY(newChild.NodeCountBody() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));
        }

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

        Transform oldMainTransform = _oldMainNode.Body.transform;

        Sequence oldMainSequence = DOTween.Sequence()
            .Append(oldMainTransform.DOMoveY((cardAmount + 1) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing))
            .AppendInterval(_horizontalTime + _waitTime)
            .Append(oldMainTransform.DOMoveZ(2.5f, _horizontalTime).SetEase(_horizontalEasing))
            .Append(oldMainTransform.DOMoveY((cardAmount + 1 - mainCardsAmount) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_verticalEasing));


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

    }
}