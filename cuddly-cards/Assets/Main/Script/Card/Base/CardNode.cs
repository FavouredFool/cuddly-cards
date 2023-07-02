/**
 * TreeNode.cs
 * Author: Luke Holland (http://lukeholland.me/)
 */

using System.Collections.Generic;
using UnityEngine;
using static CardInfo;

public class CardNode
{
	public delegate bool TraversalNodeDelegate(CardNode node);

	private readonly CardContext _context;
	private CardBody _body;

	private CardNode _parent;
	private List<CardNode> _children;

	private bool _isTopLevel;

	public CardNode(CardContext context)
	{
		_context = context;
		_children = new List<CardNode>();
	}

	public CardNode(CardContext context, CardNode parent) : this(context)
	{
		_parent = parent;
	}

	public CardContext Context { get { return _context; } }
	public CardBody Body { set { _body = value; } get { return _body; } }
	public CardNode Parent { set { _parent = value; } get { return _parent; } }
	public List<CardNode> Children { get { return _children; } }
	public bool IsTopLevel { set { _isTopLevel = value; } get { return _isTopLevel; } }

	public CardNode this[int key]
	{
		get { return _children[key]; }
	}

	public void AddChild(CardNode node)
	{
		node.Parent = this;

		_children.Add(node);
	}

	public int SetPositionsRecursive(int cardsBelowCardAndParent)
    {
		if (IsTopLevel)
        {
			return 0;
        }

		Body.transform.position = new Vector3(Parent.Body.transform.position.x, Parent.Body.transform.position.y - cardsBelowCardAndParent * CardInfo.CARDHEIGHT, Parent.Body.transform.position.z);

		cardsBelowCardAndParent = 1;

		for (int i = 0; i < Children.Count; i++)
        {
			cardsBelowCardAndParent += Children[i].SetPositionsRecursive(cardsBelowCardAndParent);
        }

		return cardsBelowCardAndParent;
	}

	public void TraverseChildren(CardTraversal traversal, TraversalNodeDelegate handler)
	{
		if (!handler(this))
		{
			return;
		}

		foreach (CardNode child in _children)
		{
			if (traversal == CardTraversal.BODY && child.IsTopLevel)
            {
				continue;
            }

			child.TraverseChildren(traversal, handler);
		}
	}

	public List<CardNode> GetTopNodesBelowNodeInPile(CardNode topOfPile, CardTraversal traversal)
	{
		List<CardNode> addedCards = new();

		if (this == topOfPile)
		{
			return addedCards;
		}

		for (int i = _parent._children.IndexOf(this) + 1; i < _parent._children.Count; i++)
		{
			if (traversal == CardTraversal.BODY && _parent._children[i].IsTopLevel)
            {
				continue;
            }

			addedCards.Add(_parent._children[i]);
		}

		addedCards.AddRange(_parent.GetTopNodesBelowNodeInPile(topOfPile, traversal));

		return addedCards;
	}

	public CardNode GetTopLevelNode()
    {
		if (!_isTopLevel)
        {
			return Parent.GetTopLevelNode();
        }

		return this;
    }

	public int GetNodeCountBelowNodeInPile(CardNode topOfPile, CardTraversal traversal)
	{
		List<CardNode> nodes = GetTopNodesBelowNodeInPile(topOfPile, traversal);

		int nodeCount = 0;

		foreach (CardNode node in nodes)
        {
			nodeCount += node.GetNodeCount(traversal);
        }

		return nodeCount;
	}

	public int GetNodeCountUpToNodeInPile(CardNode topOfPile, CardTraversal traversal)
    {
		return GetNodeCount(traversal) + GetNodeCountBelowNodeInPile(topOfPile, traversal);
    }

	public int GetNodeCount(CardTraversal traversal)
	{
		int nodeCount = 1;

		foreach (CardNode child in _children)
		{
			if (traversal == CardTraversal.BODY && child.IsTopLevel)
            {
				continue;
            } 

			nodeCount += child.GetNodeCount(traversal);
		}

		return nodeCount;
	}
}