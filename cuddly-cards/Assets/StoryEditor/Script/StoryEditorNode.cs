using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoryEditorNode
{
	public delegate bool TraversalNodeDelegate(StoryEditorNode node);

	public StoryEditorNode()
	{
		Children = new List<StoryEditorNode>();
	}

	public StoryEditorNode(StoryEditorNode parent) : this()
	{
		Parent = parent;
	}

	public StoryEditorBody Body { set; get; }
	public StoryEditorNode Parent { set; get; }
	public List<StoryEditorNode> Children { get; }
	public StoryEditorNode this[int key] => Children[key];


	public void AddChild(StoryEditorNode node)
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

		foreach (StoryEditorNode child in Children)
		{
			child.TraverseChildren(handler);
		}
	}
}
