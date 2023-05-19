using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    [SerializeField, Range(-45, 45)]
    float _cardTableRotation = 0;

    [SerializeField, Range(-45, 45)]
    float _closeUpRotation = 0;

    public void Start()
    {
        SetCardTableRotation();
    }

    public void SetCardTableRotation()
    {
        transform.localRotation = Quaternion.Euler(_cardTableRotation, 0, 0);
    }

    public void SetCloseUpRotation()
    {
        transform.localRotation = Quaternion.Euler(_closeUpRotation, 0, 0);
    }
}