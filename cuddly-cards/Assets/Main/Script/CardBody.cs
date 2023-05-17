using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CardBody : MonoBehaviour
{
    [SerializeField]
    TMP_Text _textField;

    CardBody _parentBody;
    List<CardBody> _childrenBody;

    public CardBody ParentBody { set { _parentBody = value; } get { return _parentBody; } }
    public List<CardBody> ChildrenBody { set { _childrenBody = value; } get { return _childrenBody; } }


    public void Awake()
    {
        _childrenBody = new();
    }


    public void SetLabel(string labelText)
    {
        _textField.text = labelText;
    }

    public void SetHeight(int height)
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            CardInfo.CARDHEIGHT * height,
            transform.localPosition.z
        );
    }

    public int BodyCount()
    {
        int nodeCount = 1;

        foreach (CardBody child in ChildrenBody)
        {
            nodeCount += child.BodyCount();
        }

        return nodeCount;
    }


}