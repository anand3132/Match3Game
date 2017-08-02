using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator : MonoBehaviour {
	public GameObject dummyNode;
	public GameObject nodePocket;

	public GameObject pockets;
	public const int ROWS = 8;
	public const int COLS = 8;

	public NodePocket pickedPocket1 = null;
	public NodePocket pickedPocket2 = null;

	public List<NodePocket> tempListForDebug = new List<NodePocket> ();

	void Start() {
		GeneratePockets (ROWS, COLS);
		Link (pockets);
	}

	void GeneratePockets(int row, int col) {
		Color[] presetColors = {
			Color.blue
			, Color.cyan
			, Color.green
			, Color.magenta
			, Color.red
		};
		for (int y = 0; y < row; y++) {
			for (int x = 0; x < col; x++) {
				// Pocket creation
				GameObject pocket = (GameObject)Instantiate(nodePocket);
				Bounds bounds = pocket.GetComponent<BoxCollider2D> ().bounds;
				pocket.GetComponent<NodePocket> ().SetColor (new Color(0.3f, 0.3f, 0.3f, 0.3f));
				pocket.transform.position = Vector3.zero;	// This is a must else the child node won't position properly.

				// Node creation
				GameObject node = (GameObject)Instantiate(dummyNode);
				node.transform.SetParent (pocket.transform);
				node.transform.position = Vector3.zero;
				int randIndex = Random.Range (0, presetColors.Length);
				Color clr = presetColors [randIndex];
				node.GetComponent<Node> ().SetColor (clr);

				// Pocket positioning
				// This has to come after we made the node a child. Else the child node won't position properly.
				pocket.transform.position = new Vector3 (x * bounds.size.x, y * bounds.size.y, 0);
				pocket.transform.SetParent (pockets.transform);
			}
		}
	}

	void Link(GameObject rootPocketNode) {
		for (int itr = 0; itr < rootPocketNode.transform.childCount; itr++) {
			int row = itr / COLS;
			int col = itr % COLS;
			NodePocket pocket = rootPocketNode.transform.GetChild (itr).GetComponent<NodePocket> ();
			pocket.tileIndex = itr;

			bool border = true;

			// inner nodes
			if (row > 0 && row < ROWS - 1 && col > 0 && col < COLS - 1) {
				pocket.south = rootPocketNode.transform.GetChild ((row - 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.north = rootPocketNode.transform.GetChild ((row + 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.east = rootPocketNode.transform.GetChild (row * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.west = rootPocketNode.transform.GetChild (row * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.southEast = rootPocketNode.transform.GetChild ((row-1) * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.northEast = rootPocketNode.transform.GetChild ((row+1) * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.northWest = rootPocketNode.transform.GetChild ((row+1) * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.southWest = rootPocketNode.transform.GetChild ((row-1) * COLS + col - 1).transform.GetComponent<NodePocket> ();
				border = false;
			}


			// left bottom
			if (row == 0 && col == 0) {
				pocket.east = rootPocketNode.transform.GetChild (row * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.northEast = rootPocketNode.transform.GetChild ((row+1) * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.north = rootPocketNode.transform.GetChild ((row + 1) * COLS + col).transform.GetComponent<NodePocket>();
				border = false;
			}

			// left top
			if (row == ROWS - 1 && col == 0) {
				pocket.east = rootPocketNode.transform.GetChild (row * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.southEast = rootPocketNode.transform.GetChild ((row-1) * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.south = rootPocketNode.transform.GetChild ((row - 1) * COLS + col).transform.GetComponent<NodePocket>();
				border = false;
			}

			// right bottom
			if (row == 0 && col == COLS - 1) {
				pocket.west = rootPocketNode.transform.GetChild (row * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.northWest = rootPocketNode.transform.GetChild ((row+1) * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.north = rootPocketNode.transform.GetChild ((row + 1) * COLS + col).transform.GetComponent<NodePocket>();
				border = false;
			}

			// right top
			if (row == ROWS - 1 && col == COLS - 1) {
				pocket.south = rootPocketNode.transform.GetChild ((row - 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.southWest = rootPocketNode.transform.GetChild ((row-1) * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.west = rootPocketNode.transform.GetChild (row * COLS + col - 1).transform.GetComponent<NodePocket> ();
				border = false;
			}

			// left border
			if (col == 0 && border) {
				pocket.north = rootPocketNode.transform.GetChild ((row + 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.south = rootPocketNode.transform.GetChild ((row - 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.east = rootPocketNode.transform.GetChild (row * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.southEast = rootPocketNode.transform.GetChild ((row-1) * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.northEast = rootPocketNode.transform.GetChild ((row+1) * COLS + col + 1).transform.GetComponent<NodePocket> ();
			}

			// right border
			if (col == COLS - 1 && border) {
				pocket.north = rootPocketNode.transform.GetChild ((row + 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.south = rootPocketNode.transform.GetChild ((row - 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.west = rootPocketNode.transform.GetChild (row * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.northWest = rootPocketNode.transform.GetChild ((row+1) * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.southWest = rootPocketNode.transform.GetChild ((row-1) * COLS + col - 1).transform.GetComponent<NodePocket> ();
			}

			// top border
			if (row == ROWS - 1 && border) {
				pocket.south = rootPocketNode.transform.GetChild ((row - 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.east = rootPocketNode.transform.GetChild (row * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.west = rootPocketNode.transform.GetChild (row * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.southEast = rootPocketNode.transform.GetChild ((row-1) * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.southWest = rootPocketNode.transform.GetChild ((row-1) * COLS + col - 1).transform.GetComponent<NodePocket> ();
			}

			// bottom border
			if (row == 0 && border) {
				pocket.north = rootPocketNode.transform.GetChild ((row + 1) * COLS + col).transform.GetComponent<NodePocket>();
				pocket.east = rootPocketNode.transform.GetChild (row * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.west = rootPocketNode.transform.GetChild (row * COLS + col - 1).transform.GetComponent<NodePocket> ();
				pocket.northEast = rootPocketNode.transform.GetChild ((row+1) * COLS + col + 1).transform.GetComponent<NodePocket> ();
				pocket.northWest = rootPocketNode.transform.GetChild ((row+1) * COLS + col - 1).transform.GetComponent<NodePocket> ();
			}
		}
	}

	void TraverseNode(NodePocket pocket, Color clr, NodePocket.DIRECTION dir, List<NodePocket> result) {
		if (pocket == null)
			return;
		if (pocket.GetColor () == clr) {
			AddPocket (pocket, result);
			TraverseNode (pocket.GetPocket(dir), clr, dir, result);
		}
	}

	void TraverseNode(NodePocket pocket, Color clr, List<NodePocket> result) {
		if (pocket == null)
			return;
		if (pocket.GetColor () != clr)
			return;

		AddPocket (pocket, result);

		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.NORTH), clr, NodePocket.DIRECTION.NORTH, result );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.SOUTH), clr, NodePocket.DIRECTION.SOUTH, result );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.EAST), clr, NodePocket.DIRECTION.EAST, result );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.WEST), clr, NodePocket.DIRECTION.WEST, result );
//		TraverseNode (node.GetNode(Node.NODE_DIRECTION.NORTH_EAST), clr, Node.NODE_DIRECTION.NORTH_EAST, result );
//		TraverseNode (node.GetNode(Node.NODE_DIRECTION.NORTH_WEST), clr, Node.NODE_DIRECTION.NORTH_WEST, result );
//		TraverseNode (node.GetNode(Node.NODE_DIRECTION.SOUTH_EAST), clr, Node.NODE_DIRECTION.SOUTH_EAST, result );
//		TraverseNode (node.GetNode(Node.NODE_DIRECTION.SOUTH_WEST), clr, Node.NODE_DIRECTION.SOUTH_WEST, result );
	}

	void AddPocket(NodePocket pocket, List<NodePocket> result) {
		if (pocket.visitationCount == 0) {
			result.Add (pocket);
			pocket.visitationCount++;
		}
	}

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			pickedPocket1 = null;
			pickedPocket2 = null;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);
			if (hit) {
				GameObject pickedObject = hit.collider.gameObject;
				NodePocket pocket = pickedObject.GetComponent<NodePocket> ();
				if (pocket) {
					pickedPocket1 = pocket;
				}
			}
		} else if (Input.GetMouseButtonUp (0) && pickedPocket1) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);
			if (hit) {
				GameObject pickedObject = hit.collider.gameObject;
				NodePocket pocket = pickedObject.GetComponent<NodePocket> ();
				if (pocket && pickedPocket1.IsAdjasentPocket (pocket)) {	// If the second tile is adjasent tile
					pickedPocket2 = pocket;

					// try swap
					NodePocket.SwapNodes(pickedPocket1, pickedPocket2);
					Debug.Log ("Swap "+ pickedPocket1.tileIndex + ", "+ pickedPocket2.tileIndex);

					// try traverse and find matches
					tempListForDebug.Clear();
					TraverseNode(pickedPocket1, pickedPocket1.GetColor(), tempListForDebug);
					TraverseNode(pickedPocket2, pickedPocket2.GetColor(), tempListForDebug);
				} else {
					pickedPocket1 = null;	// if not adjasent tile then reset the vars.
					pickedPocket2 = null;
				}
			}
		}
	}
}
