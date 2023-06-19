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

    public void UpdateObjects(List<CopyObject> copyObjectList, List<CardNode> topLevelCards)
    {
        copyObjectList.ForEach(e => e.gameObject.SetActive(false));
        _cameraCopyTransform.SetCopyTransform(_mainCameraTransform);

        for (int i = 0; i < topLevelCards.Count; i++)
        {
            copyObjectList[i].gameObject.SetActive(true);
            copyObjectList[i].GetComponent<TransformCopy>().SetCopyTransform(topLevelCards[i].Body.transform);
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
