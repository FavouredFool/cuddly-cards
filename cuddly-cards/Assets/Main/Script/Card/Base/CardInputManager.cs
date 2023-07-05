using UnityEngine;
using System.Collections.Generic;

public class CardInputManager : MonoBehaviour
{    
    List<Collider> _colliders;

    public CardManager CardManager { get; set; }

    public void Awake()
    {
        _colliders = new List<Collider>();
    }

    public void Update()
    {
        HoverCard();

        if (Input.GetMouseButtonDown(0))
        {
            EvaluateClick(CardInfo.Click.LEFT);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            EvaluateClick(CardInfo.Click.RIGHT);
        }
        
    }

    public void HoverCard()
    {
        CardManager.NodeHovered(GetHoveredCard());
    }

    public void EvaluateClick(CardInfo.Click click)
    {
        // note that the card can be null to express that a click has happened without a target card
        CardManager.NodeClicked(GetHoveredCard(), click);
    }

    public CardNode GetHoveredCard()
    {
        Ray shotRay = CardManager.Camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(shotRay, out RaycastHit hit)) return null;
        if (hit.collider == null) return null;
        if (!_colliders.Contains(hit.collider)) return null;

        return CardManager.GetClickableNodes()[_colliders.IndexOf(hit.collider)];
    }

    public void SetColliders()
    {
        if (_colliders.Count != 0)
        {
            RemoveColliders();
        }

        foreach (CardNode node in CardManager.GetClickableNodes())
        {
            BoxCollider nodeCollider = gameObject.AddComponent<BoxCollider>();

            float totalHeight = node.GetNodeCount(CardInfo.CardTraversal.BODY) * CardInfo.CARDHEIGHT;

            nodeCollider.center = node.Body.transform.position - new Vector3(0, totalHeight / 2f, 0);
            nodeCollider.size = new Vector3(1f, totalHeight, CardInfo.CARDRATIO);

            _colliders.Add(nodeCollider);
        }
    }

    public void RemoveColliders()
    {
        foreach (Collider nodeCollider in _colliders)
        {
            Destroy(nodeCollider);
        }

        _colliders.Clear();
    }
}