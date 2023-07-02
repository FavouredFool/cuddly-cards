using UnityEngine;
using System.Collections.Generic;

public class CardInputManager : MonoBehaviour
{    
    List<Collider> _colliders;

    CardManager _cardManager;

    public CardManager CardManager { get { return _cardManager; } set { _cardManager = value; } }

    public void Awake()
    {
        _colliders = new();
    }

    public void Update()
    {
        HoverCard();

        if (Input.GetMouseButtonDown(0))
        {
            EvaluateClick();
        }
    }

    public void HoverCard()
    {
        _cardManager.NodeHovered(GetHoveredCard());
    }

    public void EvaluateClick()
    {
        // note that the card can be null to express that a click has happened without a target card
        _cardManager.NodeClicked(GetHoveredCard());
    }

    public CardNode GetHoveredCard()
    {
        Ray shotRay = _cardManager.Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(shotRay, out RaycastHit hit))
        {
            if (hit.collider == null)
            {
                return null;
            }

            if (!_colliders.Contains(hit.collider))
            {
                return null;
            }

            return _cardManager.GetClickableNodes()[_colliders.IndexOf(hit.collider)];
        }

        return null;
    }

    public void SetColliders()
    {
        if (_colliders.Count != 0)
        {
            RemoveColliders();
        }

        foreach (CardNode node in _cardManager.GetClickableNodes())
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();

            float totalHeight = node.GetNodeCount(CardInfo.CardTraversal.BODY) * CardInfo.CARDHEIGHT;

            collider.center = node.Body.transform.position - new Vector3(0, totalHeight / 2f, 0);
            collider.size = new Vector3(1f, totalHeight, CardInfo.CARDRATIO);

            _colliders.Add(collider);
        }
    }

    public void RemoveColliders()
    {
        foreach (BoxCollider collider in _colliders)
        {
            Destroy(collider);
        }

        _colliders.Clear();
    }
}