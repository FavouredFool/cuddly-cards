using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEInputManager : MonoBehaviour
{
    [SerializeField]
    Camera _camera;

    SENodeManager _manager;

    public void Awake()
    {
        _manager = GetComponent<SENodeManager>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SENode node = GetHoveredNode();

            if (node == null) return;

            EvaluateNode(node);
        }
    }

    public void EvaluateNode(SENode node)
    {
        if (node == _manager.RootNode)
        {
            return;
        }

        if (node == _manager.BaseNode)
        {
            _manager.SetBaseNode(node.Parent);
        }
        else
        {
            _manager.SetBaseNode(node);
        }

        
    }

    public SENode GetHoveredNode()
    {
        Ray shotRay = _camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(shotRay, out RaycastHit hit)) return null;
        if (hit.collider == null) return null;

        SEBody body = hit.collider.gameObject.GetComponent<SEBody>();

        if (body == null) return null;

        return body.ReferenceNode;
    }
}
