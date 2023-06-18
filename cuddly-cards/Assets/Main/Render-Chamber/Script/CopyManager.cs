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

    [SerializeField]
    List<TransformCopy> _copyList;

    [SerializeField]
    List<Transform> _cards;

    List<CopyObject> _copyObjectList;

    private void Start()
    {
        _copyObjectList = _copyList.Select(e => e.GetComponent<CopyObject>()).ToList();
    }

    void Update()
    {
        _cameraCopyTransform.SetCopyTransform(_mainCameraTransform);
        
        for (int i = 0; i < _copyList.Count; i++)
        {
            _copyList[i].SetCopyTransform(_cards[i].transform);
        }
    }

    public void ActivateCopyObject(int index)
    {
        for (int i = 0; i < _copyList.Count; i++)
        {
            _copyObjectList[i].CopyObjectRenderer.enabled = i == index;
        }
    }
}
