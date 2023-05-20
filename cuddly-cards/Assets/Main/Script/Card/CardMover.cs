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
    Ease _easing;

    CardManager _cardManager;

    public void Awake()
    {
        _cardManager = GetComponent<CardManager>();
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

    public void MoveCardOld(CardNode card, Vector2 position)
    {
        card.Body.transform.localPosition = new Vector3(position.x, card.Body.transform.localPosition.y, position.y);
    }

    public void MoveCardsForLayoutOld(CardNode mainNode, CardNode rootNode)
    {
        MoveCardOld(mainNode, new Vector2(-2.5f, 0));

        if (mainNode != rootNode)
        {
            MoveCardOld(mainNode.Parent, new Vector2(-2.5f, 2.5f));

            if (mainNode.Parent != rootNode)
            {
                MoveCardOld(rootNode, new Vector2(2.875f, 2.5f));
            }
        }

        for (int i = 0; i < mainNode.Children.Count; i++)
        {
            MoveCardOld(mainNode.Children[i], new Vector2(i * 1.125f - 1f, 0));
        }
    }

    public void MoveCardsForLayout(CardNode activeNode, CardNode oldActiveNode, CardNode rootNode)
    {
        CardNode mainNode = activeNode;
        CardNode newBackNode = activeNode.Parent;
        CardNode newDiscardNode = newBackNode != rootNode && newBackNode != null ? rootNode : null;
        List<CardNode> newChildNodes = activeNode.Children;

        CardNode oldMainNode = oldActiveNode;
        CardNode oldBackNode = oldActiveNode.Parent;
        CardNode oldDiscardNode = oldBackNode != rootNode && oldBackNode != null ? rootNode : null;
        List<CardNode> oldChildNodes = oldActiveNode.Children;

        int cardAmount = 0;
        int mainCardsAmount;

        // Das hier ist alles nur tiefer in den kartenstack (auf child klicken)
        // Discard pile
        HandleDiscard(oldDiscardNode, oldBackNode);



        for(int i = newChildNodes.Count - 1; i >= 0; i--)
        {
            CardNode newChild = newChildNodes[i];
            cardAmount += newChild.NodeCountBody();

            // Vertikal nach oben
            newChild.Body.transform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing)
                // Zusammen mit der OldMainCard reinfahren
                .OnComplete(() => { newChild.Body.transform.DOMoveX(-2.5f, _horizontalTime).SetEase(_easing)
                    // Warten
                    .OnComplete(()=> { newChild.Body.transform.DOMove(newChild.Body.transform.position, _waitTime).SetEase(_easing)
                        // Rausfahren
                        .OnComplete(()=> { newChild.Body.transform.DOMoveX( newChild.Parent.Children.IndexOf(newChild) * 1.125f - 1f, _horizontalTime).SetEase(_easing)
                             // Runterlassen
                             .OnComplete(() =>{ newChild.Body.transform.DOMoveY(newChild.NodeCountBody() * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing);
                }); }); }); });
        }

        cardAmount += mainNode.NodeCountBody();

        // Vertikal nach oben
        mainNode.Body.transform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing)
            // Zusammen mit Children reinfahren
            .OnComplete(() => { mainNode.Body.transform.DOMoveX(-2.5f, _horizontalTime).SetEase(_easing)
               // Warten
               .OnComplete(() => { mainNode.Body.transform.DOMove(mainNode.Body.transform.position, _waitTime + _horizontalTime).SetEase(_easing)
                    // Runter
                    .OnComplete(() => { mainNode.Body.transform.DOMoveY(CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing);
            }); }); });

        mainCardsAmount = cardAmount;

        for (int i = oldChildNodes.Count-1; i >= 0; i--)
        {
            CardNode oldChild = oldChildNodes[i];

            if (oldChild == mainNode)
            {
                continue;
            }

            cardAmount += oldChild.NodeCountBody();

            // Vertikal nach oben
            oldChild.Body.transform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing)
                // Reinfahren
                .OnComplete(() => { oldChild.Body.transform.DOMoveX(-2.5f, _horizontalTime).SetEase(_easing)
                    // Neu parenten
                    .OnComplete(()=> { oldChild.Body.transform.parent = oldActiveNode.Body.transform;
            }); });
        }

        // Vertikal nach oben
        oldActiveNode.Body.transform.DOMoveY((cardAmount + 1) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing)
            // warten
            .OnComplete(()=> { oldActiveNode.Body.transform.DOMove(oldActiveNode.Body.transform.position, _horizontalTime + _waitTime).SetEase(_easing)
                // nach hinten
                .OnComplete(() => { oldActiveNode.Body.transform.DOMoveZ(2.5f, _horizontalTime).SetEase(_easing)
                    // vertikal nach unten
                    .OnComplete(() => {  oldActiveNode.Body.transform.DOMoveY(oldActiveNode.Body.transform.position.y - mainCardsAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing);
            }); }); });


        // Timer over one second
        // MAKE SURE THAT THE TIMER IS A BIT LONGER THAN THE MAXIMUM TWEENING TIME
        transform.DOMove(transform.position, _horizontalTime*2 + _verticalTime*2 + _waitTime + 0.1f).OnComplete(() => { _cardManager.FinishLayout(); });
    }

    public void HandleDiscard(CardNode oldDiscardNode, CardNode oldBackNode)
    {
        // this is wrong and doesnt work

        if (oldDiscardNode != null)
        {
            oldBackNode.TraverseBodyUnparent();


            // vertikal nach oben
            oldDiscardNode.Body.transform.DOMoveY((oldDiscardNode.NodeCountBody() + oldBackNode.NodeCountBody()) * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing);

            int cardAmount = oldBackNode.NodeCountBodyRightSide() + oldBackNode.NodeCountBody();


            // vertikal nach oben
            oldBackNode.Body.transform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, _verticalTime).SetEase(_easing)
                // wait
                .OnComplete(() => { oldBackNode.Body.transform.DOMove(oldBackNode.Body.transform.position, _horizontalTime + _waitTime).SetEase(_easing)
                    // horizontal
                    .OnComplete(() => { oldBackNode.Body.transform.DOMoveX(2.875f, _horizontalTime).SetEase(_easing);
                    }); });
            

        }
        else
        {
            if (oldBackNode != null)
            {
                // Wait
                oldBackNode.Body.transform.DOMove(oldBackNode.Body.transform.position, _verticalTime + _horizontalTime + _waitTime)
                    // horizontal
                    .OnComplete(() => { oldBackNode.Body.transform.DOMoveX(2.875f, _horizontalTime).SetEase(_easing);
                    });

                
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
}