using UnityEngine;
using System.Collections.Generic;

public class CardInput : MonoBehaviour
{
    List<BoxCollider> _colliders;

    public void Awake()
    {
        _colliders = new();
    }

    public void UpdateColliders(List<CardNode> topLevelNodes)
    {
        foreach (BoxCollider collider in _colliders)
        {
            Destroy(collider);
        }

        foreach (CardNode node in topLevelNodes)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();

            float totalHeight = node.NodeCount() * CardInfo.CARDHEIGHT;

            collider.center = node.Body.transform.position - new Vector3(0, totalHeight / 2f, 0);
            collider.size = new Vector3(1f, totalHeight, CardInfo.CARDRATIO);

            _colliders.Add(collider);
        }
    }
}