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
	private readonly CardNode _parentNode;
	private readonly int _level;
	private readonly List<CardNode> _childrenNode;

	private CardBody _cardBody;

	public CardNode(CardContext context)
	{
		_context = context;
		_childrenNode = new List<CardNode>();
		_level = 0;
	}

	public CardNode(CardContext context, CardNode parent) : this(context)
	{
		_parentNode = parent;
		_level = _parentNode != null ? _parentNode.Level + 1 : 0;
	}

	public CardContext Context { get { return _context; } }
	public CardBody Body { set { _cardBody = value; } get { return _cardBody; } }
	public CardNode ParentNode { get { return _parentNode; } }
	public List<CardNode> ChildrenNode { get { return _childrenNode; } }
	public int Level { get { return _level; } }

	public CardNode this[int key]
	{
		get { return _childrenNode[key]; }
	}


	public void Clear()
	{
		_childrenNode.Clear();
	}

	public CardNode AddChild(CardContext value)
	{
		CardNode node = new CardNode(value, this);
		_childrenNode.Add(node);

		return node;
	}

	public bool HasChild(CardContext context)
	{
		return FindInChildren(context) != null;
	}

	public CardNode FindInChildren(CardContext context)
	{
		for (int i = 0; i < _childrenNode.Count; ++i)
		{
			CardNode child = _childrenNode[i];
			if (child.Context.Equals(context)) return child;
		}

		return null;
	}

	public bool RemoveChild(CardNode node)
	{
		return _childrenNode.Remove(node);
	}

	public int NodeCount()
    {
		int nodeCount = 1;

		foreach (CardNode node in _childrenNode)
        {
			nodeCount += node.NodeCount();
        }

		return nodeCount;
    }

	public void Traverse(TraverseNodeDelegate handler)
	{
		if (!handler(this))
        {
			return;
        }

		foreach (CardNode node in _childrenNode)
		{
			node.Traverse(handler);
		}
	}

	public int TraverseHeight(TraverseNodeHeightDelegate handler, int height)
	{
		handler(this, height);

		height = 0;	

		foreach (CardNode node in _childrenNode)
        {
			height -= 1;
			height += node.TraverseHeight(handler, height);
		}

		return height;
	}

}