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

    public void Start()
    {
        _closeUpCanvas.SetActive(false);
    }

    public void EnterCloseUp(CardNode closeUpNode)
    {
        _currentNode = closeUpNode;
        _cameraMovement.SetCloseUpRotation(_closeUpRotation, _transitionTime, _easing);

        

        Vector3 goalPosition = _cardCloseUpTransform.position;
        Quaternion goalRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(_closeUpRotation, 0, 0) * Quaternion.Euler(-90, 0, 0);

        _currentNode.Body.transform.DOMove(goalPosition, _transitionTime).SetEase(_easing);
        _currentNode.Body.transform.DORotateQuaternion(goalRotation, _transitionTime).SetEase(_easing);

    }

    public void DisplayElements()
    {
        //_currentNode.Body.transform.SetPositionAndRotation(_cardCloseUpTransform.position, Quaternion.identity);
        //_currentNode.Body.transform.rotation = Quaternion.Euler(180, 180, 180) * _cameraMovement.transform.rotation * Quaternion.Euler(-90, 0, 0);

        _closeUpCanvas.SetActive(true);
        _descriptionText.text = _currentNode.Context.GetDescription();
    }

    public void ExitCloseUp()
    {
        _closeUpCanvas.SetActive(false);
        _cameraMovement.SetCardTableRotation(_transitionTime, _easing);

        // Make position dynamic
        _currentNode.Body.transform.DOMove(new Vector3(-2.5f, 0, 0), _transitionTime).SetEase(_easing);
        _currentNode.Body.transform.DORotateQuaternion(Quaternion.identity, _transitionTime).SetEase(_easing);
    }

    public void CloseUpFinished()
    {
        
        _cardManager.SetLayout();
    }
}