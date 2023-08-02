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

    public async Task SetCloseUpAnimated(CardNode closeUpNode, CloseUpStyle style, CardManager cardManager, DialogueContext dialogueContext, bool flipRight)
    {
        _cameraMovement.SetCloseUpRotation(_closeUpRotation, _transitionTime, _easing);

        Vector3 endPosition = _cardCloseUpTransform.position;
        Quaternion endRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(_closeUpRotation, 0, 0) * Quaternion.Euler(-90, 0, 0);

        closeUpNode.Body.transform.DOMove(endPosition, _transitionTime).SetEase(_easing);
        await closeUpNode.Body.transform.DORotateQuaternion(endRotation, _transitionTime).SetEase(_easing).AsyncWaitForCompletion();

        if (style == CloseUpStyle.DIALOGUE)
        {
            await Flip(closeUpNode, dialogueContext.Name, cardManager.CardBuilder.GetPersonImageFromCard(), flipRight);
        }
    }

    public async Task RevertCloseUpAnimated(CardNode closeUpNode, Vector3 originalPosition, Quaternion originalRotation, CloseUpStyle style, CardManager cardManager, bool flipRight)
    {
        _closeUpCanvas.SetActive(false);
        
        if (style == CloseUpStyle.DIALOGUE)
        {
            await Flip(closeUpNode, closeUpNode.Context.Label, cardManager.CardBuilder.GetOriginalImageFromCard(closeUpNode), flipRight);
        }

        _cameraMovement.SetCardTableRotation(_transitionTime, _easing);

        closeUpNode.Body.transform.DOMove(originalPosition, _transitionTime).SetEase(_easing);
        await closeUpNode.Body.transform.DORotateQuaternion(originalRotation, _transitionTime).SetEase(_easing).AsyncWaitForCompletion();
    }

    public async Task Flip(CardNode node, string label, Sprite icon, bool flipRight)
    {
        string newLabel = label;
        Sprite newIcon = icon;

        node.Body.SetBackElements(newLabel, newIcon);

        Quaternion startRotation = node.Body.transform.rotation;
        Quaternion addedRotation = flipRight ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, -180);
        await node.Body.transform.DORotateQuaternion(startRotation * addedRotation, _rotationTime).SetEase(_easing).AsyncWaitForCompletion();
        node.Body.transform.rotation = startRotation;

        node.Body.ClearBack();
        node.Body.SetFrontElements(newLabel, newIcon);
    }

    public void SetText(string text)
    {
        DescriptionText.text = text;
    }
}