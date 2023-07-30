using UnityEngine;
using DG.Tweening;


public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    float _cardTableRotation = 0;

    [SerializeField]
    CloseUpManager _closeUpManager;



    public void SetCardTableRotation(float transitionTime, Ease easing)
    {
        transform.DORotate(new Vector3(_cardTableRotation, 0, 0), transitionTime, RotateMode.Fast).SetEase(easing);
    }

    public void SetCloseUpRotation(float closeUpRotation, float transitionTime, Ease easing)
    {
        transform.DORotate(new Vector3(closeUpRotation, 0, 0), transitionTime, RotateMode.Fast).SetEase(easing);
    }

    public float GetCardTableRotation()
    {
        return _cardTableRotation;
    }
}