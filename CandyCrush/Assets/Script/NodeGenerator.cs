using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeGenerator : MonoBehaviour {
	public GameObject dummyNode;
	public GameObject nodePocket;

	public GameObject pockets;
	public const int ROWS = 8;
	public const int COLS = 8;
	public int regenerateCount=0;
	public NodePocket pickedPocket1 = null;
	public NodePocket pickedPocket2 = null;

//	public List<NodePocket> tempListForDebug = new List<NodePocket> ();
	public List<NodePocket> northSouthListA = new List<NodePocket> ();
	public List<NodePocket> eastWestListA = new List<NodePocket> ();
	public List<NodePocket> northSouthListB = new List<NodePocket> ();
	public List<NodePocket> eastWestListB = new List<NodePocket> ();
	public List<NodePocket> destructionList = new List<NodePocket> ();
	public List<NodePocket> dropAnimationList = new List<NodePocket> ();

	Node.TYPE[] presetTypes = {
		Node.TYPE.NODE_0,
		Node.TYPE.NODE_1,
		Node.TYPE.NODE_2,
		Node.TYPE.NODE_3,
		Node.TYPE.NODE_4
	};

	void Start() {
		GeneratePockets (ROWS, COLS);
		Link (pockets);	
		CalculateDebugText (pockets);
		//TraverseGrid (pockets);
	}

	public void Reset() {
		Node [] nodes = pockets.GetComponentsInChildren <Node> ();
		foreach (Node n in nodes) {
			n.transform.SetParent (null);
			GameObject.DestroyImmediate (n.gameObject);
		}
		GenerateNodes ();
		CalculateDebugText (pockets);
	}

	void GenerateNodes() {
		NodePocket [] nodePockets = pockets.GetComponentsInChildren <NodePocket> ();
		foreach(NodePocket p in nodePockets) {
			Vector3 oldPos = p.transform.position;
			p.transform.position = Vector3.zero;	// This is a must else the child node won't position properly.

			// Node creation
			GameObject node = (GameObject)Instantiate(dummyNode);
			node.transform.SetParent (p.transform);
			node.transform.position = Vector3.zero;
			int randIndex = Random.Range (0, presetTypes.Length);
			node.GetComponent<Node> ().SetState (Node.STATE.ACTIVE);
			node.GetComponent<Node> ().SetNodeType (presetTypes[randIndex]);

			// Pocket positioning
			// This has to come after we made the node a child. Else the child node won't position properly.
			p.transform.position = oldPos;
		}
	}

	void GeneratePockets(int row, int col) {
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
				node.GetComponent<Node> ().SetState (Node.STATE.ACTIVE);
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
			NodePocket pocket = rootPocketNode.transform.GetChild (itr).GetComponent<NodePocket> ();
			pocket.CalculateDebugText ();
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

	void AddPocket(NodePocket pocket, List<NodePocket> result, bool force = false) {
		if (pocket.visitationCount == 0 || force) {
			result.Add (pocket);
			// Debug.Log ("visitation " + pocket.tileIndex / COLS + ", " + pocket.tileIndex % COLS);
			pocket.visitationCount++;
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
		if (pocket.GetNode () && (pocket.GetNode().GetNodeType() != type || pocket.GetNode ().GetState () != Node.STATE.ACTIVE))
			return;

		AddPocket (pocket, ns, true);
		AddPocket (pocket, ew, true);

		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.NORTH), type, NodePocket.DIRECTION.NORTH, ns );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.SOUTH), type, NodePocket.DIRECTION.SOUTH, ns );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.EAST), type, NodePocket.DIRECTION.EAST, ew );
		TraverseNode (pocket.GetPocket(NodePocket.DIRECTION.WEST), type, NodePocket.DIRECTION.WEST, ew );
//		Debug.Log ("visitation count"+  pocket.visitationCount);

	}
		
	void TraverseGrid(GameObject rootPocketNode){
		for (int itr = 0; itr < rootPocketNode.transform.childCount; itr++) {
			NodePocket pocket = rootPocketNode.transform.GetChild (itr).GetComponent<NodePocket> ();
			ClearArrays();
			TraverseNode(pocket, pocket.GetNode().GetNodeType(), northSouthListA, eastWestListA);
			ClearVisitationFlag ();
			if (!isMatchFound()) {
				//Debug.Log ("Match Not found!!");
			} else {
				DestroyNodesFromDestructionList();
				//fall animation 
				//move north pocket to (itr)position;
				//regenarate destroyed object on top
			}
		}
	}

	public void CallbackDropDownAnimComplete(object pocket) {
		List<NodePocket> list = (List<NodePocket>)pocket;
		Debug.Log ("drop down finished " + list.Count);

		if (list.Count == 2) {
			dropAnimationList.Remove (list[0]);
			NodePocket.SwapNodes (list [0], list [1]);
			if (dropAnimationList.Count == 0) {
				DoNextMatch ();
			}
		}
	}

	bool DoNextMatch() {
		int nDestruction = 0;
		NodePocket [] nodePockets = pockets.GetComponentsInChildren <NodePocket> ();
		foreach (NodePocket p in nodePockets) {
			// try traverse and find matches
			if (p.GetNode ().GetState () != Node.STATE.ACTIVE) {
				continue;
			}
			ClearArrays ();
			TraverseNode (p, p.GetNode ().GetNodeType (), northSouthListA, eastWestListA);
			ClearVisitationFlag ();
			if (isMatchFound ()) {
				// start destruction
				BlinkNodes (destructionList);

				DestroyNodesFromDestructionList ();
				nDestruction++;
				break;
			}
		}

		if (nDestruction == 0) {
			// allow mouse input.
			Debug.LogWarning ("No Match found");
		}
		//

		return nDestruction > 0;
	}

	public void StepDropDownDebug() {
		int nDestruction = 0;
		NodePocket [] nodePockets = pockets.GetComponentsInChildren <NodePocket> ();
		foreach (NodePocket p in nodePockets) {
			// try traverse and find matches
			if (p.GetNode ().GetState () != Node.STATE.ACTIVE) {
				continue;
			}
			ClearArrays ();
			TraverseNode (p, p.GetNode ().GetNodeType (), northSouthListA, eastWestListA);
			ClearVisitationFlag ();
			if (isMatchFound ()) {
				// start destruction
				BlinkNodes (destructionList);
				// DestroyNodesFromDestructionList ();
				nDestruction++;
				break;
			}
		}

		if (nDestruction == 0) {
			Debug.LogWarning ("No Match found");
		}
	}

	public void CallbackBlinkComplete(object color) {
	}

	void BlinkNodes(List<NodePocket> list) {
		foreach(NodePocket p in list) {
			iTween.FadeTo( p.GetNode ().gameObject , 
				iTween.Hash( 
					"alpha" , 0.3f , 
					"time" , .3 , 
					"easeType", "easeInSine"
				));
		}	
	}

	NodePocket GetSouthTip(NodePocket northNodePocket) {
		while(northNodePocket.south != null) {
			northNodePocket = northNodePocket.south;
		}
		return northNodePocket;
	}
		
	void TryDropDown(NodePocket northNodePocket){
		NodePocket southNodePocket = northNodePocket.south;
		bool foundAtleastOneDestroyedNode = false;
		Vector3 bottomPosToDrop = new Vector3 ();

		while (southNodePocket) {
			if (!southNodePocket.IsActiveNode ()) {
				foundAtleastOneDestroyedNode = true;
			}
			if (foundAtleastOneDestroyedNode && southNodePocket.IsActiveNode ()) {
				bottomPosToDrop = southNodePocket.transform.position;
				bottomPosToDrop.y += northNodePocket.gameObject.GetComponent<BoxCollider2D> ().bounds.size.y;
				// southNodePocket.SetColor (Color.white);
				break;
			}
			southNodePocket = southNodePocket.south;
		}

		NodePocket southTip = null;
		if (southNodePocket) {
			southTip = (southNodePocket.north) ? southNodePocket.north : GetSouthTip (northNodePocket);
		} else {
			southTip = GetSouthTip (northNodePocket);
		}

		float yPos = bottomPosToDrop.y;
		while (northNodePocket) {
			if (!IsExistInList (northNodePocket, dropAnimationList)) {
				dropAnimationList.Add (northNodePocket);
				//northNodePocket.SetColor (Color.white);
				List<NodePocket> list = new List<NodePocket>();
				list.Add (northNodePocket);
				list.Add (southTip);
				iTween.MoveTo(northNodePocket.GetNode ().gameObject, 
					iTween.Hash(
						"y", yPos,
						"time", 1.0f,
						"oncomplete", "CallbackDropDownAnimComplete",
						"onCompleteParams", list,
						"oncompletetarget", this.gameObject
					));
				//Debug.Log ("dropping : y pos "+yPos + " pocket "+northNodePocket.ToString ());
				yPos += northNodePocket.gameObject.GetComponent<BoxCollider2D> ().bounds.size.y;
			}
			southTip = southTip.north;
			northNodePocket = northNodePocket.north;
		}

		if (dropAnimationList.Count > 0) {
			Debug.Log ("Start drop animation " + dropAnimationList.Count);
		} else {
			// start the check again
			DoNextMatch ();
		}
	}

	void DestroyNodesFromDestructionList() {
		Debug.Log ("Start destruction "+ destructionList.Count);
		regenerateCount = destructionList.Count;
		foreach (NodePocket p in destructionList) {
//			 Debug.Log ("drop "+p.ToString ());
			p.DestroyNode ();
		}

		bool bTryDropAtleastOne = false;
		foreach (NodePocket p2 in destructionList) {
			NodePocket northNodePocket = p2.north;
			while (northNodePocket) {
				bool found = IsExistInList (northNodePocket, destructionList);
				if (found == false) {
					bTryDropAtleastOne = true;
//					Debug.Log ("Try drop "+northNodePocket.ToString ());
					TryDropDown (northNodePocket);
					break;
				}
				northNodePocket = northNodePocket.north;
			}
		}

		if (destructionList.Count > 0 && !bTryDropAtleastOne) {
			// start the check again
			DoNextMatch ();
		}
	}

	void ClearArrays() {
		northSouthListA.Clear ();
		eastWestListA.Clear ();
		northSouthListB.Clear ();
		eastWestListB.Clear ();
		destructionList.Clear ();
		dropAnimationList.Clear ();
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


	bool IsExistInList(NodePocket node, List<NodePocket> list) {
		foreach(NodePocket p in list) {
			if (p.tileIndex == node.tileIndex) {
				//Debug.Log ("Found "+p.ToString ());
				return true;
			}
		}
		return false;
	}

	bool isMatchFound(){
		bool MatchFound = false;
		if (northSouthListA.Count >= 3) {
			ChangeColor (northSouthListA, Color.yellow);
			destructionList.AddRange (northSouthListA);
			MatchFound= true;
		}
		if (eastWestListA.Count >= 3) {
			ChangeColor (eastWestListA, Color.yellow);
			destructionList.AddRange (eastWestListA);
			MatchFound= true;
		}
		if (northSouthListB.Count >= 3) {
			ChangeColor (northSouthListB, Color.yellow);
			destructionList.AddRange (northSouthListB);
			MatchFound= true;
		}
		if (eastWestListB.Count >= 3) {
			ChangeColor (eastWestListB, Color.yellow);
			destructionList.AddRange (eastWestListB);
			MatchFound= true;
		}
		return MatchFound;
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
				if (pocket && pocket.GetNode ().GetState ()==Node.STATE.ACTIVE) {
					pickedPocket1 = pocket;
				}
			}
		} else if (Input.GetMouseButtonUp (0) && pickedPocket1) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);
			if (hit) {
				GameObject pickedObject = hit.collider.gameObject;
				NodePocket pocket = pickedObject.GetComponent<NodePocket> ();
				if (pocket && pickedPocket1.IsAdjasentPocket (pocket)&& pocket.GetNode ().GetState ()==Node.STATE.ACTIVE) {	// If the second tile is adjasent tile
					pickedPocket2 = pocket;

					// try swap
					NodePocket.SwapNodes(pickedPocket1, pickedPocket2);

					// try traverse and find matches
					ClearArrays();
					TraverseNode(pickedPocket1, pickedPocket1.GetNode().GetNodeType(), northSouthListA, eastWestListA);
					ClearVisitationFlag ();
					TraverseNode(pickedPocket2, pickedPocket2.GetNode().GetNodeType(), northSouthListB, eastWestListB);
					ClearVisitationFlag ();

					if (!isMatchFound()) {
						NodePocket.SwapNodes (pickedPocket1, pickedPocket2);
						Debug.Log("No Match Found Swap Back"); 
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
