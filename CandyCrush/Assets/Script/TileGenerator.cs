using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile{
	//pointer to actual tile
	public GameObject tileObject;
	public string id;
	public int col,row;
	public Tile leftNode;
	public Tile rightNode;
	public Tile topNode;
	public Tile BottomNode;
	public Tile topRightNode;
	public Tile topLeftNode;
	public Tile BottomRightNode;
	public Tile BottomLeftNode;

	public Tile(GameObject tile,string t){
		tileObject = tile;
		id = t;
	}
}

public class TileGenerator : MonoBehaviour {


	GameObject tile1=null;
	GameObject tile2=null;

	public GameObject[] tile;
	Tile[,] tiles=new Tile[cols,rows];
	List<GameObject> tileBank = new List<GameObject> ();

	static int cols=8;
	static int rows = 8;
	int objectsToBePooled=(rows * cols) / 4;
	bool renewGrid = false;

	void Start () {
		/*
		ObjectPool (objectsToBePooled);
		ShuffleList (tileBank);
		GenerateTileGrid ();
		LinkTileGrid ();
		*/
	}

	void ObjectPool(int poolSize){
		//object pooling
		for (int i = 0; i < poolSize; i++) {
			for (int j = 0; j < tile.Length; j++) {
				GameObject o = (GameObject)Instantiate (tile [j], new Vector3 (-10, -10, 0), tile [j].transform.rotation);
				o.SetActive (false);
				tileBank.Add (o);
			}
		}
	}
	void ShuffleList ( List<GameObject> list){
		System.Random rnd = new System.Random ();
		int count = list.Count;
		while (count > 1) {
			count--;
			int n = rnd.Next (count + 1);
			GameObject tmp=list[n];
			list[n]=list[count];
			list[count]=tmp;
		}
	}
		
	void GenerateTileGrid (){
		//Initialise Tile Grid
		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < cols; c++) {
				Vector3 tilePos = new Vector3 (c, r, 0);
				for (int n = 0; n < tileBank.Count; n++) {
					GameObject obj = tileBank [n];
					if (!obj.activeSelf) {
						obj.transform.position = new Vector3 (tilePos.x, tilePos.y, tilePos.z);
						obj.SetActive (true);
						//assign node for each tile game object created
						tiles[c, r] = new Tile (obj, obj.name);
						tiles [c, r].col = c;
						tiles [c, r].row = r;
						n = tileBank.Count + 1;
					}
				}
			}
		}
	}

	void LinkTileGrid(){
		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < cols; c++) {
				
				if (c == 0) { 
					tiles [c, r].topLeftNode = null;
					tiles [c, r].leftNode = null;
					tiles [c, r].BottomLeftNode = null;
				} else {
					tiles [c, r].leftNode = tiles [c-1, r];
					if (r == 0) {
						tiles [c, r].BottomLeftNode = null;
					} else {
						tiles [c, r].BottomLeftNode = tiles [c - 1, r - 1];
					}
					if (r == rows - 1) {
						tiles [c, r].topLeftNode = null;
					} else {
						tiles [c, r].topLeftNode = tiles [c - 1, r + 1];
					}
				}

				if (c == cols - 1) {
					tiles [c, r].topRightNode = null;
					tiles [c, r].rightNode = null;
					tiles [c, r].BottomRightNode = null;				
				} else {
						tiles [c, r].topRightNode = tiles [c + 1, r + 1];
						tiles [c, r].rightNode = tiles [c+1, r ];
					Debug.Log (" C : " + c + " R :" + r+"/col "+tiles [c , r ].col+"/row "+tiles [c , r ].row);
					if (r == 0) {
						tiles [c, r].BottomRightNode = null;
					} else {
						tiles [c, r].BottomRightNode = tiles [c + 1, r + 1];
					}
				}

				if (r == 0) {
					tiles [c, r].BottomNode = null;
				} else {
					tiles [c, r].BottomNode = tiles [c , r-1];

				}

				if (r == rows - 1) {	
					tiles [c, r].topNode = null;
				} else {
					tiles [c, r].topNode = tiles [c , r+1];

				}	
			}//for (col)
		}//for (row)
	}//LinkTileGrid()

	// Update is called once per frame
	void Update () {

		/*
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);
			if (hit) {
				tile1 = hit.collider.gameObject;


			}
		} else if(Input.GetMouseButtonUp(0)&&tile1) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection (ray, 1000);
			if (hit) {
				tile2 = hit.collider.gameObject;
			}

			if (tile1 && tile2) {
				
				int tile1posX = (int)tile1.transform.position.x;
				int tile1posY = (int)tile1.transform.position.y;
				int tile2posX = (int)tile2.transform.position.x;
				int tile2posY = (int)tile2.transform.position.y;
					
				int horDist = (int)Mathf.Abs (tile1posX - tile2posX);
				int verDist = (int)Mathf.Abs (tile1posY - tile2posY);

				if (horDist == 1 ^ verDist == 1) {//no digonal swap
					Tile tmp=tiles[tile1posX,tile1posY];
					tiles[tile1posX,tile1posY]=tiles[tile2posX,tile2posY];
					tiles[tile2posX,tile2posY]=tmp;

					
				Vector3 tempPos = tile1.transform.position;
				tile1.transform.position = tile2.transform.position;
				tile2.transform.position = tempPos;
					Debug.Log("Colum "+ tiles [tile1posX, tile1posY].col+"Row "+
						tiles [tile1posX, tile1posY].row);
				LinkTileGrid ();
				tile1 = null;
				tile2 = null;

				}
			}
		}
		MatchTile ();
		*/
	}
	void MatchTile(){
	//	int counter = 1;
		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < cols; c++) {
				if(tiles[c,r].rightNode!=null){
					if (tiles [c, r].id == tiles [c, r].rightNode.id) {
						if (tiles [c, r].leftNode != null) {
							if (tiles [c, r].id == tiles [c, r].rightNode.id) {
								if (tiles [c, r].rightNode.rightNode!=null){
									if (tiles [c, r].id == tiles [c, r].rightNode.rightNode.id) {
										tiles [c + 2, r].tileObject.SetActive (false);
									}
								}
								if (tiles [c, r].leftNode.leftNode != null) {
									if (tiles [c, r].id == tiles [c, r].leftNode.leftNode.id) {
										tiles [c -2, r].tileObject.SetActive (false);
									}
								}
								tiles [c+1, r].tileObject.SetActive (false);
								tiles [c, r].tileObject.SetActive (false);
								tiles [c-1, r].tileObject.SetActive (false);
							}
						}
					}
						
				}
//					if (tiles [c, r].leftNode != null) {
//					}
//					if (tiles [c, r].topNode != null) {
//					}
//					if (tiles [c, r].BottomNode != null) {
//					}
//					if (tiles [c, r].topLeftNode != null) {
//					}
//					if (tiles [c, r].BottomRightNode != null) {
//					}
//					if (tiles [c, r].topRightNode != null) {
//					}
//					if (tiles [c, r].BottomLeftNode != null) {
//					}
			}//for (col)
		}//for (row)
	}//MatchTile()
		
	void RenewGrid (){
		bool tileMoved = false;
		ShuffleList (tileBank);
		for (int r = 1; r < rows; r++) {
			for (int c = 0; c < cols; c++) {
				if (r == rows - 1 && tiles [c, r] != null ) {
					Vector3 tilePos = new Vector3 (c, r, 0);
					for (int n = 0; n < tileBank.Count; n++) {
						GameObject o = tileBank [n];
						if (!o.activeSelf) {
							o.transform.position = new Vector3 (tilePos.x, tilePos.y, tilePos.z);
							o.SetActive (true);
							tiles [c, r] = new Tile (o, o.name);
							n = tileBank.Count + 1;
						}
					}
				}
				if (tiles [c, r] != null) {
					//drop down bellow if space is empty
					if (tiles [c, r - 1] == null) {
						tiles [c, r - 1] = tiles [c, r];
						tiles [c, r - 1].tileObject.transform.position = new Vector3 (c, r - 1, 0);
						tiles [c, r] = null;
						tileMoved = true;

					}
				}
			}
		}
		if (tileMoved) {
			Invoke ("RenewGrid", 1f);
		}
	}//RenewGrid ()
}//TileGenerator()
