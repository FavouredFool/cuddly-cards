using UnityEngine;



public class CloseUpState : LayoutState
{
    StateManager _manager;
    CardNode _closeUpNode;

    public CloseUpState(StateManager manager, CardNode clickedNode)
    {
        _manager = manager;
        _closeUpNode = clickedNode;
    }

    public void StartState()
    {
        CloseUpManager closeUpManager = _manager.GetCloseUpManager();

        CameraMovement camMovement = closeUpManager.GetCameraMovement();
        camMovement.transform.rotation = Quaternion.Euler(new Vector3(closeUpManager.GetCloseUpRotation(), 0, 0));

        Vector3 endPosition = closeUpManager.GetCloseUpTransform().position;
        Quaternion endRotation = Quaternion.Euler(180, 180, 180) * Quaternion.Euler(closeUpManager.GetCloseUpRotation(), 0, 0) * Quaternion.Euler(-90, 0, 0);

        _closeUpNode.Body.transform.position = endPosition;
        _closeUpNode.Body.transform.rotation = endRotation;

        closeUpManager.GetCloseUpCanvas().SetActive(true);
        closeUpManager.GetDescriptionText().text = _closeUpNode.Context.GetDescription();
    }

    public void HandleClick(CardNode clickedNode)
    {
        // get out of the thingy
        CloseUpManager closeUpManager = _manager.GetCloseUpManager();

        closeUpManager.GetCloseUpCanvas().SetActive(false);
        closeUpManager.GetCameraMovement().transform.rotation = Quaternion.Euler(closeUpManager.GetCameraMovement().GetCardTableRotation(), 0, 0);

        _manager.PopState();
    }
}
