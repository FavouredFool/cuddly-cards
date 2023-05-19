using UnityEngine;

public class CardCloseUpManager : MonoBehaviour
{
    [SerializeField]
    CameraMovement _cameraMovement;

    [SerializeField]
    Transform _cardCloseUpTransform;

    public void EnterCloseUp(CardNode closeUpNode)
    {
        _cameraMovement.SetCloseUpRotation();

        closeUpNode.Body.transform.SetPositionAndRotation(_cardCloseUpTransform.position, Quaternion.identity);
        closeUpNode.Body.transform.rotation = Quaternion.Euler(180, 180, 180) * _cameraMovement.transform.rotation * Quaternion.Euler(-90, 0, 0);

        // text
    }

    public void ExitCloseUp()
    {
        _cameraMovement.SetCardTableRotation();
    }
}