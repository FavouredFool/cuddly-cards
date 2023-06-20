using UnityEngine;

public class ModelTransform : MonoBehaviour
{
    public void SetModelTransform(Transform objectTransform)
    {
        transform.SetLocalPositionAndRotation(objectTransform.position, objectTransform.rotation);
    }
}
