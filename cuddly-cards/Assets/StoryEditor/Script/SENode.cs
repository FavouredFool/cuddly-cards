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

	public SENode(SEContext context, SENode parent) : this(context)
	{
		Parent = parent;
	}

	public SEContext Context { set; get; }
	public SEBody Body { set; get; }
	public SENode Parent { set; get; }
	public List<SENode> Children { get; }
	public SENode this[int key] => Children[key];


	public void AddChild(SENode node)
	{
		node.Parent = this;
		Children.Add(node);
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
