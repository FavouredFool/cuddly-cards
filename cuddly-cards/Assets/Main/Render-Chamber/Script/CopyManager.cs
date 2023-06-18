using System.Collections.Generic;
using UnityEngine;

public class CopyManager : MonoBehaviour
{
    [SerializeField]
    Transform _mainCameraTransform;

    [SerializeField]
    TransformCopy _cameraCopy;

    [SerializeField]
    List<TransformCopy> _copyList;

    [SerializeField]
    List<Transform> _cards;

    void Update()
    {
        _cameraCopy.SetCopyTransform(_mainCameraTransform);

        for (int i = 0; i < _copyList.Count; i++)
        {
            _copyList[i].SetCopyTransform(_cards[i].transform);
        }
    }
}
