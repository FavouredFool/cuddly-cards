using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SENode
{
	public delegate bool TraversalNodeDelegate(SENode node);

	public SENode(SEContext objectElement)
	{
		SEObjectElement = objectElement;
		Children = new List<SENode>();
	}

	[SerializeField, HideInInspector]
	public SEContext SEObjectElement;

	public SEBody Body { set; get; }
	public SENode Parent { set; get; }
	public List<SENode> Children { get; }
	public SENode this[int key] => Children[key];

	public void AddChild(SENode node)
    {
		AddChild(node, SEObjectElement.Depth + 1);
    }
	public void AddChild(SENode node, int depth)
	{
		node.Parent = this;
		Children.Add(node);
		node.SEObjectElement.Depth = depth;
	}

	public void TraverseChildren(TraversalNodeDelegate handler)
	{
		if (!handler(this))
		{
			return;
		}

		foreach (SENode child in Children)
		{
			child.TraverseChildren(handler);
		}
	}
}
