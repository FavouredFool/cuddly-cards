/**
 * TreeNode.cs
 * Author: Luke Holland (http://lukeholland.me/)
 */

using System.Collections.Generic;

public class CardNode
{
	public delegate bool TraversalNodeDelegate(CardNode node);

	private readonly CardContext _context;
	private CardBody _body;
	
	private int _level;

	private CardNode _parent;
	private List<CardNode> _children;

	private bool _isTopLevel;

	public CardNode(CardContext context)
	{
		_context = context;
		_children = new List<CardNode>();
		_level = 0;
	}

	public CardNode(CardContext context, CardNode parent) : this(context)
	{
		_parent = parent;
		_level = _parent != null ? _parent.Level + 1 : 0;
	}

	public int Level { set { _level = value; } get { return _level; } }
	public CardContext Context { get { return _context; } }
	public CardBody Body { set { _body = value; } get { return _body; } }
	public CardNode Parent { set { _parent = value; } get { return _parent; } }
	public List<CardNode> Children { get { return _children; } }
	public bool IsTopLevel { set { _isTopLevel = value; } get { return _isTopLevel; } }

	public CardNode this[int key]
	{
		get { return _children[key]; }
	}

	public void Clear()
	{
		_children.Clear();
	}

	public void AddChild(CardNode node)
	{
		node.Parent = this;
        node.Level = Level + 1;

		_children.Add(node);
	}

	public void TraverseContext(TraversalNodeDelegate handler)
	{
		if (!handler(this))
		{
			return;
		}

		foreach (CardNode child in _children)
		{
			child.TraverseContext(handler);
		}
	}

	public void TraverseBody(TraversalNodeDelegate handler)
	{
		if (!handler(this))
		{
			return;
		}

		foreach (CardNode child in _children)
		{
			if (child.IsTopLevel)
			{
				continue;
			}

			child.TraverseBody(handler);
		}
	}

	public int NodeCount()
    {
		int nodeCount = 1;

		foreach (CardNode child in _children)
        {
			if (child.IsTopLevel)
			{
				continue;
			}

			nodeCount += child.NodeCount();
        }

		return nodeCount;
    }


	public int SetHeightRecursive(int height)
	{
		Body.SetHeight(height);

		height = 0;

		foreach (CardNode child in Children)
		{
			if (child.IsTopLevel)
			{
				continue;
			}

			height -= 1;
			height += child.SetHeightRecursive(height);
		}

		return height;
	}

}