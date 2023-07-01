using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class CloseUpManager : MonoBehaviour
{
    [SerializeField]
    CameraMovement _cameraMovement;

    [SerializeField]
    Transform _cardCloseUpTransform;

    [SerializeField]
    GameObject _closeUpCanvas;

    [SerializeField]
    CardManager _cardManager;

    [SerializeField]
    TMP_Text _descriptionText;

    [Header("Animation")]
    [SerializeField, Range(-45, 45)]
    float _closeUpRotation = -25;

    [SerializeField, Range(0.1f, 5)]
    float _transitionTime = 1;

    [SerializeField]
    Ease _easing;

    public void Start()
    {
        _closeUpCanvas.SetActive(false);
    }

    public void SetCloseUpStatic(CardNode closeUpNode)
    {
        _cameraMovement.transform.rotation = Quaternion.Euler(new Vector3(GetCloseUpRotation(), 0, 0));

        Vector3 endPosition = GetCloseUpTransform().position;
        Quaternion endRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(GetCloseUpRotation(), 0, 0) * Quaternion.Euler(-90, 0, 0);

        closeUpNode.Body.transform.SetPositionAndRotation(endPosition, endRotation);

        GetCloseUpCanvas().SetActive(true);
        GetDescriptionText().text = closeUpNode.Context.GetDescription();
    }

    public void RevertCloseUpStatic(CardNode closeUpNode, Vector3 originalPosition, Quaternion originalRotation)
    {
        // out animation
        GetCloseUpCanvas().SetActive(false);
        GetCameraMovement().transform.rotation = Quaternion.Euler(GetCameraMovement().GetCardTableRotation(), 0, 0);

        closeUpNode.Body.transform.SetPositionAndRotation(originalPosition, originalRotation);
    }

    public async Task SetCloseUpAnimated(CardNode closeUpNode)
    {
        _cameraMovement.SetCloseUpRotation(_closeUpRotation, _transitionTime, _easing);

        Vector3 endPosition = _cardCloseUpTransform.position;
        Quaternion endRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(_closeUpRotation, 0, 0) * Quaternion.Euler(-90, 0, 0);

        closeUpNode.Body.transform.DOMove(endPosition, _transitionTime).SetEase(_easing);
        await closeUpNode.Body.transform.DORotateQuaternion(endRotation, _transitionTime).SetEase(_easing).AsyncWaitForCompletion();
    }

    public async Task RevertCloseUpAnimated(CardNode closeUpNode, Vector3 originalPosition, Quaternion originalRotation)
    {
        _closeUpCanvas.SetActive(false);
        _cameraMovement.SetCardTableRotation(_transitionTime, _easing);
        closeUpNode.Body.transform.DOMove(originalPosition, _transitionTime).SetEase(_easing);
        await closeUpNode.Body.transform.DORotateQuaternion(originalRotation, _transitionTime).SetEase(_easing).AsyncWaitForCompletion();
    }

    public CameraMovement GetCameraMovement()
    {
        return _cameraMovement;
    }

    public float GetCloseUpRotation()
    {
        return _closeUpRotation;
    }

    public Transform GetCloseUpTransform()
    {
        return _cardCloseUpTransform;
    }

    public GameObject GetCloseUpCanvas()
    {
        return _closeUpCanvas;
    }

    public TMP_Text GetDescriptionText()
    {
        return _descriptionText;
    }
}