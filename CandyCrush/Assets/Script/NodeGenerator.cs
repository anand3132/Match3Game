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

//	public List<NodePocket> tempListForDebug = new List<NodePocket> ();
	public List<NodePocket> northSouthListA = new List<NodePocket> ();
	public List<NodePocket> eastWestListA = new List<NodePocket> ();
	public List<NodePocket> northSouthListB = new List<NodePocket> ();
	public List<NodePocket> eastWestListB = new List<NodePocket> ();
	public List<NodePocket> destructionList = new List<NodePocket> ();

	void Start() {
		GeneratePockets (ROWS, COLS);
		Link (pockets);	
		CalculateDebugText (pockets);
	}

	void GeneratePockets(int row, int col) {
		Node.TYPE[] presetTypes = {
			Node.TYPE.NODE_0,
			Node.TYPE.NODE_1,
			Node.TYPE.NODE_2,
			Node.TYPE.NODE_3,
			Node.TYPE.NODE_4
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
				int randIndex = Random.Range (0, presetTypes.Length);
				node.GetComponent<Node> ().SetNodeType (presetTypes[randIndex]);

				// Pocket positioning
				// This has to come after we made the node a child. Else the child node won't position properly.
				pocket.transform.position = new Vector3 (x * bounds.size.x, y * bounds.size.y, 0);
				pocket.transform.SetParent (pockets.transform);
			}
		}
	}

	void ChangeColor() {
		foreach(NodePocket p in northSouthListA) {
			p.GetNode ().SetDebugColor (Color.yellow);
		}
		foreach(NodePocket p in eastWestListA) {
			p.GetNode ().SetDebugColor (Color.yellow);
		}
		foreach(NodePocket p in northSouthListB) {
			p.GetNode ().SetDebugColor (Color.yellow);
		}
		foreach(NodePocket p in eastWestListB) {
			p.GetNode ().SetDebugColor (Color.yellow);
		}
	}

	void ChangeColor(List<NodePocket> array, Color clr) {
		foreach(NodePocket p in array) {
			p.GetNode ().SetDebugColor (clr);
		}
	}

	void CalculateDebugText(GameObject rootPocketNode) {
		for (int itr = 0; itr < rootPocketNode.transform.childCount; itr++) {
			int row = itr / COLS;
			int col = itr % COLS;
			NodePocket pocket = rootPocketNode.transform.GetChild (itr).GetComponent<NodePocket> ();
			Node node = pocket.gameObject.GetComponentInChildren<Node> ();
			GameObject textParent = node.gameObject.transform.GetChild (0).gameObject;
			textParent.GetComponent<TextMesh> ().text = "" + row + "," + col;
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
				//Debug.Log("(row - 1) * COLS + col)"+(row - 1) * COLS + col);
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

	void TraverseNode(NodePocket pocket, Node.TYPE type, NodePocket.DIRECTION dir, List<NodePocket> result) {
		if (pocket == null)
			return;
		if (pocket.GetNode () && pocket.GetNode().GetNodeType() == type) {
			AddPocket (pocket, result);
			TraverseNode (pocket.GetPocket(dir), type, dir, result);
		}
	}

	void TraverseNode(NodePocket pocket, Node.TYPE type, List<NodePocket> ns, List<NodePocket> ew) {
		if (pocket == null)
			return;
		if (pocket.GetNode () && pocket.GetNode().GetNodeType() != type)
			return;

		AddPocket (pocket, ns, true);
		AddPocket (pocket, ew, true);

		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.NORTH), type, NodePocket.DIRECTION.NORTH, ns );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.SOUTH), type, NodePocket.DIRECTION.SOUTH, ns );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.EAST), type, NodePocket.DIRECTION.EAST, ew );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.WEST), type, NodePocket.DIRECTION.WEST, ew );
		Debug.Log ("visitation count"+  pocket.visitationCount);

	}

	void DestroyNodesFromDestructionList() {
		foreach (NodePocket p in destructionList) {
			p.DestroyNode ();
		}
	}

	void AddPocket(NodePocket pocket, List<NodePocket> result, bool force = false) {
		if (pocket.visitationCount == 0 || force) {
			result.Add (pocket);
			// Debug.Log ("visitation " + pocket.tileIndex / COLS + ", " + pocket.tileIndex % COLS);
			pocket.visitationCount++;
		}
	}

	void ClearArrays() {
		northSouthListA.Clear ();
		eastWestListA.Clear ();
		northSouthListB.Clear ();
		eastWestListB.Clear ();
		destructionList.Clear ();
	}

	void ClearVisitationFlag() {
		foreach(NodePocket p in northSouthListA) {
			p.visitationCount = 0;
		}
		foreach(NodePocket p in eastWestListA) {
			p.visitationCount = 0;
		}
		foreach(NodePocket p in northSouthListB) {
			p.visitationCount = 0;
		}
		foreach(NodePocket p in eastWestListB) {
			p.visitationCount = 0;
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
					ClearArrays();
					TraverseNode(pickedPocket1, pickedPocket1.GetNode().GetNodeType(), northSouthListA, eastWestListA);
					ClearVisitationFlag ();
					TraverseNode(pickedPocket2, pickedPocket2.GetNode().GetNodeType(), northSouthListB, eastWestListB);
					ClearVisitationFlag ();

					Debug.Log ("northSouthListA " + northSouthListA.Count);
					Debug.Log ("eastWestListA " + eastWestListA.Count);
					Debug.Log ("northSouthListB " + northSouthListB.Count);
					Debug.Log ("eastWestListB " + eastWestListB.Count);

					bool matchFound = false;
					if (northSouthListA.Count >= 3) {
						ChangeColor (northSouthListA, Color.yellow);
						matchFound = true;
						destructionList.AddRange (northSouthListA);
					}
					if (eastWestListA.Count >= 3) {
						ChangeColor (eastWestListA, Color.yellow);
						matchFound = true;
						destructionList.AddRange (eastWestListA);
					}
					if (northSouthListB.Count >= 3) {
						ChangeColor (northSouthListB, Color.yellow);
						matchFound = true;
						destructionList.AddRange (northSouthListB);
					}
					if (eastWestListB.Count >= 3) {
						ChangeColor (eastWestListB, Color.yellow);
						matchFound = true;
						destructionList.AddRange (eastWestListB);
					}

					if (!matchFound) {
						NodePocket.SwapNodes (pickedPocket1, pickedPocket2);
						Debug.Log ("swap back");
					} else {
						// start destruction
						DestroyNodesFromDestructionList();
					}
				} else {
					pickedPocket1 = null;	// if not adjasent tile then reset the vars.
					pickedPocket2 = null;
				}
			}
		}
	}
}
