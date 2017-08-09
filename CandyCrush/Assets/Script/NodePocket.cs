using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodePocket : MonoBehaviour
{
	public NodePocket west;
	public NodePocket east;
	public NodePocket north;
	public NodePocket south;
	public NodePocket northEast;
	public NodePocket northWest;
	public NodePocket southEast;
	public NodePocket southWest;
	public int tileIndex;
	public int visitationCount = 0;

	public enum DIRECTION
	{
		NORTH,
		SOUTH,
		EAST,
		WEST,
		NORTH_EAST,
		NORTH_WEST,
		SOUTH_EAST,
		SOUTH_WEST,
		DIRECTION_MAX
	}

	public Node GetNode ()
	{
		return transform.GetComponentInChildren<Node> ();
	}

	public void SetColor (Color clr)
	{
		// if node present color the node else color the pocket
		Node node = transform.GetComponentInChildren<Node> ();
		if (node) {
			SpriteRenderer renderer = node.GetComponent<SpriteRenderer> ();
			if (renderer) {
				renderer.material.color = clr;
			}
		} else {
			SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
			if (renderer) {
				renderer.material.color = clr;
			}
		}
	}

	public Color GetColor ()
	{
		// if node present, get the color of node else get the color of pocket
		Node node = transform.GetComponentInChildren<Node> ();
		if (node) {
			SpriteRenderer renderer = node.GetComponent<SpriteRenderer> ();
			if (renderer) {
				return renderer.material.color;
			}
		} else {
			SpriteRenderer renderer = transform.GetComponent<SpriteRenderer> ();
			if (renderer) {
				return renderer.material.color;
			}
		}
		return Color.white;
	}

	public bool IsAdjasentPocket (NodePocket pocket)
	{
		if (pocket == null)
			return false;

		return (pocket == west
		|| pocket == east
		|| pocket == north
		|| pocket == south 
//				||pocket == northEast 
//				||pocket == northWest 
//				||pocket == southEast 
//				||pocket == southWest
		);
	}

	public NodePocket GetPocket (DIRECTION dir)
	{
		if (dir == DIRECTION.NORTH && IsActiveNode ()) {
			return north;
		} else if (dir == DIRECTION.SOUTH) {
			return south;
		} else if (dir == DIRECTION.EAST) {
			return east;
		} else if (dir == DIRECTION.WEST) {
			return west;
		} else if (dir == DIRECTION.NORTH_EAST) {
			return northEast;
		} else if (dir == DIRECTION.NORTH_WEST) {
			return northWest;
		} else if (dir == DIRECTION.SOUTH_EAST) {
			return southEast;
		} else if (dir == DIRECTION.SOUTH_WEST) {
			return southWest;
		}

		return null;
	}

	public static void SwapNodes (NodePocket a, NodePocket b, bool dontChangeTransform = false)
	{
		Node node1 = a.GetNode ();
		Node node2 = b.GetNode ();
		node2.gameObject.transform.SetParent (a.transform);
		if (!dontChangeTransform) {
			node2.transform.localPosition = Vector3.zero;
		}
		node1.gameObject.transform.SetParent (b.transform);
		if (!dontChangeTransform) {
			node1.transform.localPosition = Vector3.zero;
		}

		a.CalculateDebugText ();
		b.CalculateDebugText ();
	}

	public void CalculateDebugText ()
	{
		int row = tileIndex / NodeGenerator.COLS;
		int col = tileIndex % NodeGenerator.COLS;
		Node node = gameObject.GetComponentInChildren<Node> ();
		GameObject textParent = node.gameObject.transform.GetChild (0).gameObject;
		textParent.GetComponent<TextMeshPro> ().text = "" + row + "," + col;
	}

	public void DestroyNode ()
	{
		if (GetNode () == null) {
			Debug.Log ("Error at" + tileIndex / NodeGenerator.COLS + ", " + tileIndex % NodeGenerator.COLS);
		}
		GetNode ().SetState (Node.STATE.DESTROYED);
	}

	public override string ToString ()
	{
		return "r : " + tileIndex / NodeGenerator.COLS + ", c : " + tileIndex % NodeGenerator.COLS + " id : " + tileIndex;
	}

	public bool IsActiveNode ()
	{
		return GetNode ().GetState () == Node.STATE.ACTIVE;
	}
}