using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class CloseUpManager : MonoBehaviour
{
    public enum CloseUpStyle { CLOSEUP, DIALOGUE }

    [SerializeField]
    CameraMovement _cameraMovement;

    [SerializeField]
    Transform _cardCloseUpTransform;

    [SerializeField]
    GameObject _closeUpCanvas;

    [SerializeField]
    TMP_Text _descriptionText;

    [Header("Animation")]
    [SerializeField]
    float _closeUpRotation = -25;

    [SerializeField, Range(0.1f, 5)]
    float _transitionTime = 1;

    [SerializeField]
    float _rotationTime = 1;

    [SerializeField]
    Ease _easing;

    public CameraMovement CameraMovement => _cameraMovement;
    public float CloseUpRotation => _closeUpRotation;
    public Transform CloseUpTransform => _cardCloseUpTransform;
    public GameObject CloseUpCanvas => _closeUpCanvas;
    public TMP_Text DescriptionText => _descriptionText;

    public void Start()
    {
        _closeUpCanvas.SetActive(false);
    }

    public void SetCloseUpStatic(CardNode closeUpNode, CloseUpStyle style)
    {
        _cameraMovement.transform.rotation = Quaternion.Euler(new Vector3(CloseUpRotation, 0, 0));

        Vector3 endPosition = CloseUpTransform.position;
        Quaternion endRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(CloseUpRotation, 0, 0) * Quaternion.Euler(-90, 0, 0);
        if (style == CloseUpStyle.DIALOGUE) endRotation *= Quaternion.Euler(0, 0, 180);

        closeUpNode.Body.transform.SetPositionAndRotation(endPosition, endRotation);

        CloseUpCanvas.SetActive(true);
    }

    public void RevertCloseUpStatic(CardNode closeUpNode, Vector3 originalPosition, Quaternion originalRotation, CloseUpStyle style)
    {
        // out animation
        CloseUpCanvas.SetActive(false);
        CameraMovement.transform.rotation = Quaternion.Euler(CameraMovement.GetCardTableRotation(), 0, 0);

        closeUpNode.Body.transform.SetPositionAndRotation(originalPosition, originalRotation);
    }

    public async Task SetCloseUpAnimated(CardNode closeUpNode, CloseUpStyle style)
    {
        _cameraMovement.SetCloseUpRotation(_closeUpRotation, _transitionTime, _easing);

        Vector3 endPosition = _cardCloseUpTransform.position;
        Quaternion endRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(_closeUpRotation, 0, 0) * Quaternion.Euler(-90, 0, 0);

        closeUpNode.Body.transform.DOMove(endPosition, _transitionTime).SetEase(_easing);
        await closeUpNode.Body.transform.DORotateQuaternion(endRotation, _transitionTime).SetEase(_easing).AsyncWaitForCompletion();

        if (style == CloseUpStyle.DIALOGUE)
        {
            await closeUpNode.Body.transform.DORotateQuaternion(endRotation * Quaternion.Euler(0, 0, 180), _rotationTime).SetEase(_easing).AsyncWaitForCompletion();
        }

    }

    public async Task RevertCloseUpAnimated(CardNode closeUpNode, Vector3 originalPosition, Quaternion originalRotation, CloseUpStyle style)
    {
        _closeUpCanvas.SetActive(false);
        
        if (style == CloseUpStyle.DIALOGUE)
        {
            Quaternion rotation = closeUpNode.Body.transform.rotation * Quaternion.Euler(0, 0, 180);
            await closeUpNode.Body.transform.DORotateQuaternion(rotation, _rotationTime).SetEase(_easing).AsyncWaitForCompletion();
        }

        _cameraMovement.SetCardTableRotation(_transitionTime, _easing);

        closeUpNode.Body.transform.DOMove(originalPosition, _transitionTime).SetEase(_easing);
        await closeUpNode.Body.transform.DORotateQuaternion(originalRotation, _transitionTime).SetEase(_easing).AsyncWaitForCompletion();
    }

    public void SetText(string text)
    {
        DescriptionText.text = text;
    }
}