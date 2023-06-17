using UnityEngine;
using System.Collections.Generic;

public class ObjectCopy : MonoBehaviour
{
    [SerializeField]
    Vector3 _offset;

    public MeshRenderer Object;

    public void SetCopyTransform(Transform objectTransform)
    {
        transform.SetPositionAndRotation(objectTransform.position + _offset, objectTransform.rotation);
    }
}
