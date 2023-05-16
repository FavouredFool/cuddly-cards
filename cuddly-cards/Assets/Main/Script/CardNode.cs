/**
 * TreeNode.cs
 * Author: Luke Holland (http://lukeholland.me/)
 */

using System;
using System.Collections.Generic;

public class CardNode
{
	public delegate bool TraverseNodeDelegate(CardNode node);
	public delegate void TraverseNodeHeightDelegate(CardNode node, int height);

	private readonly CardContext _context;
	private readonly CardNode _parent;
	private readonly int _level;
	private readonly List<CardNode> _children;

	private CardBody _cardBody;

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

	public int Level { get { return _level; } }
	public int Count { get { return _children.Count; } }
	public bool IsRoot { get { return _parent == null; } }
	public bool IsLeaf { get { return _children.Count == 0; } }
	public CardContext Context { get { return _context; } }
	public CardBody Body { set { _cardBody = value; } get { return _cardBody; } }
	public CardNode Parent { get { return _parent; } }
	public List<CardNode> Children { get { return _children; } }

	public CardNode this[int key]
	{
		get { return _children[key]; }
	}


	public void Clear()
	{
		_children.Clear();
	}

	public CardNode AddChild(CardContext value)
	{
		CardNode node = new CardNode(value, this);
		_children.Add(node);

		return node;
	}

	public bool HasChild(CardContext context)
	{
		return FindInChildren(context) != null;
	}

	public CardNode FindInChildren(CardContext context)
	{
		int i = 0, l = Count;
		for (; i < l; ++i)
		{
			CardNode child = _children[i];
			if (child.Context.Equals(context)) return child;
		}

		return null;
	}

	public bool RemoveChild(CardNode node)
	{
		return _children.Remove(node);
	}

	public void Traverse(TraverseNodeDelegate handler)
	{
		if (!handler(this))
        {
			return;
        }

		foreach (CardNode node in _children)
		{
			node.Traverse(handler);
		}
	}

	public int TraverseHeight(TraverseNodeHeightDelegate handler, int height)
	{
		handler(this, height);

		height = 0;	

		foreach (CardNode node in _children)
        {
			height -= 1;
			height += node.TraverseHeight(handler, height);
		}

		return height;
	}

}