using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    CardNode _currentNode;

    Vector3 _originalPosition;

    bool _enterAnimationFinished;

    bool _initialCloseUp;

    public void Start()
    {
        _closeUpCanvas.SetActive(false);
    }

    public void EnterCloseUp(CardNode closeUpNode)
    {
        _enterAnimationFinished = false;

        _originalPosition = closeUpNode.Body.transform.position;

        _currentNode = closeUpNode;
        _initialCloseUp = !_currentNode.Context.GetHasBeenSeen();
        _currentNode.Context.SetHasBeenSeen(true);

        _cameraMovement.SetCloseUpRotation(_closeUpRotation, _transitionTime, _easing);

        Vector3 endPosition = _cardCloseUpTransform.position;
        Quaternion endRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(_closeUpRotation, 0, 0) * Quaternion.Euler(-90, 0, 0);

        _currentNode.Body.transform.DOMove(endPosition, _transitionTime).SetEase(_easing).OnComplete(() => { DisplayElements(); });
        _currentNode.Body.transform.DORotateQuaternion(endRotation, _transitionTime).SetEase(_easing);
    }

    public void DisplayElements()
    {
        _enterAnimationFinished = true;
        _closeUpCanvas.SetActive(true);
        _descriptionText.text = _currentNode.Context.GetDescription();
    }

    public void ExitCloseUp()
    {
        _closeUpCanvas.SetActive(false);
        _cameraMovement.SetCardTableRotation(_transitionTime, _easing);

        _currentNode.Body.transform.DOMove(_originalPosition, _transitionTime).SetEase(_easing).OnComplete(() => { CloseUpFinished(); });
        _currentNode.Body.transform.DORotateQuaternion(Quaternion.identity, _transitionTime).SetEase(_easing);
    }

    public void CloseUpFinished()
    {
        _currentNode.Body.transform.position = _originalPosition;

        _cardManager.CloseUpFinished(_initialCloseUp);
    }

    public bool GetEnterAnimationFinished()
    {
        return _enterAnimationFinished;
    }
}