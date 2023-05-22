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
            if (_cardManager.GetIsCloseUp())
            {
                _cardManager.ExitCloseUp();
            }

            // there shouldn't be any colliders around when up close, therefore no guard
            EvaluateClickedCard();
        }
    }

    public void EvaluateClickedCard()
    {
        Ray shotRay = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(shotRay, out hit))
        {
            if (hit.collider == null)
            {
                return;
            }

            if (!_colliders.Contains(hit.collider))
            {
                return;
            }

            CardNode hitNode = _cardManager.GetTopLevelNodes()[_colliders.IndexOf(hit.collider)];

            if (hitNode == _cardManager.GetActiveNode() && !_cardManager.GetIsStartLayout())
            {
                _cardManager.EnterCloseUp();
            }
            else
            {
                _cardManager.SetNodeActive(hitNode);
            }
        }
    }

    public void SetColliders()
    {
        foreach (CardNode node in _cardManager.GetTopLevelNodes())
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();

            float totalHeight = node.NodeCountBody() * CardInfo.CARDHEIGHT;

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