using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SENode
{
	public delegate bool TraversalNodeDelegate(SENode node);

	public SENode(SEContext context)
	{
		Context = context;
		Children = new List<SENode>();
	}

	public SEContext Context { set; get; }
	public SEBody Body { set; get; }
	public SENode Parent { set; get; }
	public List<SENode> Children { get; }
	public SENode this[int key] => Children[key];
	public int Depth { set; get; }


	public void AddChild(SENode node)
    {
		AddChild(node, Depth + 1);
    }
	public void AddChild(SENode node, int depth)
	{
		node.Parent = this;
		Children.Add(node);
		node.Depth = depth;
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
