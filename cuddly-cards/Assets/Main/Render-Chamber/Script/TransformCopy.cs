using UnityEngine;

public class TransformCopy : MonoBehaviour
{
    public void SetCopyTransform(Transform objectTransform)
    {
        transform.SetLocalPositionAndRotation(objectTransform.position, objectTransform.rotation);
    }
}
