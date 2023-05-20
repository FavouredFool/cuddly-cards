using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class CardMover : MonoBehaviour
{
    [SerializeField]
    Transform _cardFolder;

    CardManager _cardManager;

    //readonly float JITTERAMOUNT = 0.01f;

    public void Awake()
    {
        _cardManager = GetComponent<CardManager>();
    }

    public void ParentCards(CardNode rootNode, List<CardNode> topLevelNodes)
    {
        rootNode.TraverseContext(
            delegate (CardNode cardNode)
            {
                CardNode parent = cardNode.Parent;

                // If the node is top level, cut off any parenting
                if (topLevelNodes.Contains(cardNode)) parent = null;

                cardNode.Body.transform.parent = parent?.Body.transform;
                cardNode.Body.transform.parent ??= _cardFolder;

                return true;
            }
        );
    }

    public void PileFromParenting(CardNode topLevelNode)
    {
        if (!topLevelNode.IsTopLevel)
        {
            Debug.LogError("Tried to create pile from non-topLevel cardBody");
        }


        topLevelNode.SetHeightRecursive(0);
        topLevelNode.Body.SetHeight(topLevelNode.NodeCountBody());
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
            newChild.Body.transform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, 0.5f)
                // Zusammen mit der OldMainCard reinfahren
                .OnComplete(() => { newChild.Body.transform.DOMoveX(-2.5f, 1f)
                    // Warten
                    .OnComplete(()=> { newChild.Body.transform.DOMove(newChild.Body.transform.position, 0.5f)
                        // Rausfahren
                        .OnComplete(()=> { newChild.Body.transform.DOMoveX( newChild.Parent.Children.IndexOf(newChild) * 1.125f - 1f, 1f)
                             // Runterlassen
                             .OnComplete(() =>{ newChild.Body.transform.DOMoveY(newChild.NodeCountBody() * CardInfo.CARDHEIGHT, 0.5f);
                }); }); }); });
        }

        cardAmount += mainNode.NodeCountBody();

        // Vertikal nach oben
        mainNode.Body.transform.DOMove(new Vector3(mainNode.Body.transform.position.x, cardAmount * CardInfo.CARDHEIGHT, mainNode.Body.transform.position.z), 0.5f)
            // Zusammen mit Children reinfahren
            .OnComplete(() => { mainNode.Body.transform.DOMove(new Vector3(-2.5f, mainNode.Body.transform.position.y, 0), 1f)
               // Warten
               .OnComplete(() => { mainNode.Body.transform.DOMove(mainNode.Body.transform.position, 1.5f)
                    // Runter
                    .OnComplete(() => { mainNode.Body.transform.DOMove(new Vector3(mainNode.Body.transform.position.x, CardInfo.CARDHEIGHT, mainNode.Body.transform.position.z), 0.5f);
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
            oldChild.Body.transform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, 0.5f)
                // Reinfahren
                .OnComplete(() => { oldChild.Body.transform.DOMoveX(-2.5f, 1f)
                    // Neu parenten
                    .OnComplete(()=> { oldChild.Body.transform.parent = oldActiveNode.Body.transform;
            }); });
        }

        // Vertikal nach oben
        oldActiveNode.Body.transform.DOMoveY((cardAmount + 1) * CardInfo.CARDHEIGHT, 0.5f)
            // warten
            .OnComplete(()=> { oldActiveNode.Body.transform.DOMove(oldActiveNode.Body.transform.position, 1f)
                // nach hinten
                .OnComplete(() => { oldActiveNode.Body.transform.DOMoveZ(2.5f, 1)
                    // vertikal nach unten
                    .OnComplete(() => {  oldActiveNode.Body.transform.DOMoveY(oldActiveNode.Body.transform.position.y - mainCardsAmount * CardInfo.CARDHEIGHT, 0.5f);
            }); }); });





        /*
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

        */

        // Timer over one second
        // MAKE SURE THAT THE TIMER IS A BIT LONGER THAN THE MAXIMUM TWEENING TIME
        transform.DOMove(transform.position, 4).OnComplete(() => { _cardManager.MoveCardsFinished(); });
    }

    public void HandleDiscard(CardNode oldDiscardNode, CardNode oldBackNode)
    {
        // this is wrong and doesnt work

        if (oldDiscardNode != null)
        {
            // Back-Card gets integrated into Root-Cards if there is already a root-card
            // Therefore, the next sibling of the back-card in the node-tree needs to be a topnode

            // for each later sibling, add the sibling to top level

            // thsi does not work: What would work is if this goes recursivly to the parents and checks if there are children of the parent in the right-hand side. If so, make them toplevel. This goes rekursively if you're at the top. Then everything that's "deeper" than the Node is toplevel and will therefore not move when the rootnode moves. In a similar way the cards will need to be counted too. Just apply the counting function recursivly to the siblings to the right until the root is reached.
            /*
                oldBackNode.TraverseBodyRightSide(
                    delegate (CardNode node)
                    {
                        if (node != oldBackNode)
                        {
                            node.Body.transform.parent = null;
                        }

                        return true;
                    });
            */
            oldBackNode.TraverseBodyUnparent();


            // vertikal nach oben
            oldDiscardNode.Body.transform.DOMoveY((oldDiscardNode.NodeCountBody() + oldBackNode.NodeCountBody()) * CardInfo.CARDHEIGHT, 0.5f);

            int cardAmount = oldBackNode.NodeCountBodyRightSide() + oldBackNode.NodeCountBody();


            // vertikal nach oben
            oldBackNode.Body.transform.DOMoveY(cardAmount * CardInfo.CARDHEIGHT, 0.5f)
                // nach rechts
                .OnComplete(() => {
                    oldBackNode.Body.transform.DOMoveX(2.875f, 1);
                });
            

        }
        else
        {
            if (oldBackNode != null)
            {
                oldBackNode.Body.transform.DOMoveX(2.875f, 1);
            }
            
        }

 
        


    }

    public void ResetPositionAndRotation(CardNode rootNode)
    {
        rootNode.TraverseContext(delegate (CardNode node)
        {
            node.Body.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            return true;
        });
    }
}