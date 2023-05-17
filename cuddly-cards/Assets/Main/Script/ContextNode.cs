/**
 * TreeNode.cs
 * Author: Luke Holland (http://lukeholland.me/)
 */

using System.Collections.Generic;

public class ContextNode
{
	public delegate bool TraversalNodeDelegate(ContextNode node);

	private readonly CardContext _context;
	private readonly ContextNode _parent;
	private readonly int _level;
	private readonly List<ContextNode> _children;
	private BodyNode _cBody;

	public ContextNode(CardContext context)
	{
		_context = context;
		_children = new List<ContextNode>();
		_level = 0;
	}

	public ContextNode(CardContext data, ContextNode parent) : this(data)
	{
		_parent = parent;
		_level = _parent != null ? _parent.Level + 1 : 0;
	}

	public int Level { get { return _level; } }
	public CardContext Context { get { return _context; } }
	public ContextNode Parent { get { return _parent; } }
	public List<ContextNode> Children { get { return _children; } }
	public BodyNode CBody { set { _cBody = value; } get { return _cBody; } }

	public ContextNode this[int key]
	{
		get { return _children[key]; }
	}

	public void Clear()
	{
		_children.Clear();
	}

	public ContextNode AddChild(CardContext context)
	{
		ContextNode node = new ContextNode(context, this);
		_children.Add(node);

		return node;
	}

	public void Traverse(TraversalNodeDelegate handler)
	{
		if (!handler(this))
		{
			return;
		}

		foreach (ContextNode child in _children)
		{
			child.Traverse(handler);
		}
	}

}