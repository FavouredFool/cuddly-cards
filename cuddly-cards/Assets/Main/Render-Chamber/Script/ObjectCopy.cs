using UnityEngine;

public class ObjectCopy : MonoBehaviour
{
    [SerializeField]
    Vector3 _offset;

    Transform objectTransform;

    void Start()
    {
        objectTransform = GameObject.FindGameObjectWithTag("specialCard").transform;
    }


    void Update()
    {
        transform.SetPositionAndRotation(objectTransform.position + _offset, objectTransform.rotation);
    }
}
