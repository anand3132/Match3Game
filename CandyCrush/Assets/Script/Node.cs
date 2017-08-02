using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node:MonoBehaviour{
	public int ID;
	Vector3 nodePosition;

	public Node leftNode;
	public Node rightNode;
	public Node topNode;
	public Node BottomNode;
	public Node topRightNode;
	public Node topLeftNode;
	public Node BottomRightNode;
	public Node BottomLeftNode;

	public Node next;
	public Node(){
		nodePosition = gameObject.transform.position;
	}

}


