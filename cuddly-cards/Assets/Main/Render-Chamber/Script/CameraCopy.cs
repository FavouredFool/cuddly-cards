using UnityEngine;

public class CameraCopy : MonoBehaviour
{
    [SerializeField]
    Vector3 _offset;

    Transform cameraTransform;

    void Start()
    {   
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }


    void Update()
    {
        transform.SetPositionAndRotation(cameraTransform.position + _offset, cameraTransform.rotation);
    }
}
