using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CardInfo;

public class CardNode
{
	public delegate bool TraversalNodeDelegate(CardNode node);

	public CardNode()
    {
		Children = new List<CardNode>();
	}

    public CardNode(CardContext context)
	{
		Context = context;
		Children = new List<CardNode>();
	}

	public CardNode(CardContext context, CardNode parent) : this(context)
	{
		Parent = parent;
	}

	public CardContext Context { get; }
    public CardBody Body { set; get; }
    public CardNode Parent { set; get; }
    public List<CardNode> Children { get; }
    public bool IsTopLevel { private set; get; }
	public bool IsClickable { private set; get; }

    public CardNode this[int key] => Children[key];

	public void SetNodeState(bool isTopLevel, bool isClickable)
    {
		if (!isTopLevel && isClickable)
        {
			throw new System.Exception("node can't be clickable without being toplevel");
        }

		IsTopLevel = isTopLevel;
		IsClickable = isClickable;
    }

    public void AddChild(CardNode node)
	{
		node.Parent = this;

		Children.Add(node);
	}

	public void UnlinkFromParent()
    {
		Parent.Children.Remove(this);
		Parent = null;
	}

	public int SetPositionsRecursive(int cardsBelowCardAndParent)
    {
		if (IsTopLevel)
        {
			return 0;
        }

		Body.transform.position = new Vector3(Parent.Body.transform.position.x, Parent.Body.transform.position.y - cardsBelowCardAndParent * CardInfo.CARDHEIGHT, Parent.Body.transform.position.z);

		cardsBelowCardAndParent = 1;

		foreach (CardNode node in Children)
        {
            cardsBelowCardAndParent += node.SetPositionsRecursive(cardsBelowCardAndParent);
        }

		return cardsBelowCardAndParent;
	}

	public void SetRotationRecursive()
	{
		if (IsTopLevel)
		{
			return;
		}

		Body.transform.rotation = Parent.Body.transform.rotation;

		foreach (CardNode node in Children)
		{
			node.SetRotationRecursive();
		}
	}

	public void TraverseChildren(CardTraversal traversal, TraversalNodeDelegate handler)
    {
        if (!handler(this))
		{
			return;
		}

        foreach (CardNode child in Children.Where(child => traversal != CardTraversal.BODY || !child.IsTopLevel))
        {
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

		for (int i = Parent.Children.IndexOf(this) + 1; i < Parent.Children.Count; i++)
		{
			if (traversal == CardTraversal.BODY && Parent.Children[i].IsTopLevel)
            {
				continue;
            }

			addedCards.Add(Parent.Children[i]);
		}

		addedCards.AddRange(Parent.GetTopNodesBelowNodeInPile(topOfPile, traversal));

		return addedCards;
	}

	public CardNode GetTopLevelNode()
    {
		if (!IsTopLevel)
        {
			return Parent.GetTopLevelNode();
        }

		return this;
    }

	public int GetNodeCountBelowNodeInPile(CardNode topOfPile, CardTraversal traversal)
	{
		List<CardNode> nodes = GetTopNodesBelowNodeInPile(topOfPile, traversal);

        return nodes.Sum(node => node.GetNodeCount(traversal));
	}

	public int GetNodeCountUpToNodeInPile(CardNode topOfPile, CardTraversal traversal)
    {
		return GetNodeCount(traversal) + GetNodeCountBelowNodeInPile(topOfPile, traversal);
    }

	public int GetNodeCount(CardTraversal traversal)
    {
        return 1 + Children.Where(child => traversal != CardTraversal.BODY || !child.IsTopLevel).Sum(child => child.GetNodeCount(traversal));
    }
}