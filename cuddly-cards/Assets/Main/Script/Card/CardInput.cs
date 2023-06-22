using UnityEngine;
using System.Collections.Generic;

public class CardInput : MonoBehaviour
{
    [SerializeField]
    Camera _camera;
    
    List<Collider> _colliders;

    CardManager _cardManager;

    public void Awake()
    {
        _colliders = new();
        _cardManager = GetComponent<CardManager>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_cardManager.IsCloseUpFlag)
            {
                _cardManager.ExitCloseUp();
            }

            EvaluateClickedCard();
        }
    }

    public void EvaluateClickedCard()
    {
        Ray shotRay = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(shotRay, out RaycastHit hit))
        {
            if (hit.collider == null)
            {
                return;
            }

            if (!_colliders.Contains(hit.collider))
            {
                return;
            }

            CardNode hitNode = _cardManager.GetTopLevelNodesMainPile()[_colliders.IndexOf(hit.collider)];

            _cardManager.NodeClicked(hitNode);
        }
    }

    public void SetColliders()
    {
        foreach (CardNode node in _cardManager.GetTopLevelNodesMainPile())
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