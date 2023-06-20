using UnityEngine;

public class ModelCameraMovement : ModelTransform
{
    [SerializeField]
    Camera _camera;

    public Camera GetCamera()
    {
        return _camera;
    }
}
