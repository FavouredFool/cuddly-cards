using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CopyManager : MonoBehaviour
{
    [SerializeField]
    Transform _mainCameraTransform;

    [SerializeField]
    Camera _mainCamera;

    [SerializeField]
    TransformCopy _cameraCopyTransform;

    public void UpdateObjects(List<CopyObject> copyList, List<CardNode> topLevelCards)
    {
        _cameraCopyTransform.SetCopyTransform(_mainCameraTransform);

        for (int i = 0; i < topLevelCards.Count; i++)
        {
            copyList[i].GetComponent<TransformCopy>().SetCopyTransform(topLevelCards[i].Body.transform);
        }
    }

    public void ActivateCopyObject(List<CopyObject> copyObjectList, int index, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            copyObjectList[i].CopyObjectRenderer.enabled = i == index;
        }
    }
}
