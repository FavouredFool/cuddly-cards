/**
 * TreeNode.cs
 * Author: Luke Holland (http://lukeholland.me/)
 */

using System.Collections.Generic;

public class BodyNode
{
	public delegate bool TraversalNodeDelegate(BodyNode node);

	private readonly CardBody _body;
	private BodyNode _parent;
	private readonly int _level;
	private readonly List<BodyNode> _children;
	private ContextNode _cContext;

	public BodyNode(CardBody body)
	{
		_body = body;
		_children = new List<BodyNode>();
		_level = 0;
	}

	public BodyNode(CardBody data, BodyNode parent) : this(data)
	{
		_parent = parent;
		_level = _parent != null ? _parent.Level + 1 : 0;
	}

	public int Level { get { return _level; } }
	public CardBody Body { get { return _body; } }
	public BodyNode Parent { set { _parent = value; } get { return _parent; } }
	public List<BodyNode> Children { get { return _children; } }
	public ContextNode CContext { set { _cContext = value; } get { return _cContext; } }

	public BodyNode this[int key]
	{
		get { return _children[key]; }
	}

	public void Clear()
	{
		_children.Clear();
	}

	public BodyNode AddChild(CardBody context)
	{
		BodyNode node = new BodyNode(context, this);
		_children.Add(node);

		return node;
	}

	public void Traverse(TraversalNodeDelegate handler)
	{
		if (!handler(this))
		{
			return;
		}

		foreach (BodyNode child in _children)
		{
			child.Traverse(handler);
		}
	}

	public int NodeCount()
    {
		int nodeCount = 1;

		foreach (BodyNode child in _children)
        {
			nodeCount += child.NodeCount();
        }

		return nodeCount;
    }

}