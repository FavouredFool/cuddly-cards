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

    bool _introAnimationFinished;
    public bool IntroAnimationFinishedFlag { get { return _introAnimationFinished; } set { _introAnimationFinished = value; } }

    bool _initialCloseUp;

    public void Start()
    {
        _closeUpCanvas.SetActive(false);
    }

    public void EnterCloseUp(CardNode closeUpNode)
    {
        _introAnimationFinished = false;

        _originalPosition = closeUpNode.Body.transform.position;

        _currentNode = closeUpNode;
        _initialCloseUp = !_currentNode.Context.GetHasBeenSeen();
        _currentNode.Context.SetHasBeenSeen(true);

        IntroAnimationStart();
    }

    public void IntroAnimationStart()
    {
        _cameraMovement.SetCloseUpRotation(_closeUpRotation, _transitionTime, _easing);

        Vector3 endPosition = _cardCloseUpTransform.position;
        Quaternion endRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(_closeUpRotation, 0, 0) * Quaternion.Euler(-90, 0, 0);

        _currentNode.Body.transform.DOMove(endPosition, _transitionTime).SetEase(_easing).OnComplete(() => { IntroAnimationEnd(); });
        _currentNode.Body.transform.DORotateQuaternion(endRotation, _transitionTime).SetEase(_easing);
    }

    public void IntroAnimationEnd()
    {
        _introAnimationFinished = true;
        _closeUpCanvas.SetActive(true);
        _descriptionText.text = _currentNode.Context.GetDescription();
    }

    public void ExitCloseUp()
    {
        _closeUpCanvas.SetActive(false);
        _cameraMovement.SetCardTableRotation(_transitionTime, _easing);

        ExitAnimationStart();
    }

    public void ExitAnimationStart()
    {
        _currentNode.Body.transform.DOMove(_originalPosition, _transitionTime).SetEase(_easing).OnComplete(() => { ExitAnimationEnd(); });
        _currentNode.Body.transform.DORotateQuaternion(Quaternion.identity, _transitionTime).SetEase(_easing);
    }

    public void ExitAnimationEnd()
    {
        _currentNode.Body.transform.position = _originalPosition;

        _cardManager.CloseUpFinished(_initialCloseUp);
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