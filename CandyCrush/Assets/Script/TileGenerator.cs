using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile{
	public GameObject tileObject;
	public string type;
	public Tile(GameObject tile,string t){
		tileObject = tile;
		type = t;
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
	int objectsToBePooled=(rows * cols) / 3;
	bool renewGrid = false;

	void Start () {
		ObjectPool (objectsToBePooled);
		ShuffleList (tileBank);
		GenerateTileGrid ();
	}

	void ObjectPool(int poolSize){
		//object pooling
		for (int i = 0; i < poolSize; i++) {
			for (int j = 0; j < tile.Length; j++) {
				GameObject o = (GameObject)Instantiate (tile [j], 
					new Vector3 (-10, -10, 0), tile [j].transform.rotation);
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
					GameObject o = tileBank [n];
					if (!o.activeSelf) {
						o.transform.position = new Vector3 (tilePos.x, tilePos.y, tilePos.z);
						o.SetActive (true);
						tiles[c, r] = new Tile (o, o.name);
						n = tileBank.Count + 1;
					}
				}
			}
		}
	}
	// Update is called once per frame
	void Update () {
		
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
				tile1 = null;
				tile2 = null;
				}
			}
		}
		MatchTile ();
	}
	void MatchTile(){
		int counter = 1;
		for (int r = 0; r < rows; r++) {
			counter = 1;
			for (int c = 1; c < cols; c++) {
				if (tiles [c, r] != null && tiles [c - 1, r] != null) {
					if (tiles [c, r].type == tiles [c - 1, r].type) {
						counter++;
					} else
						counter = 1;
					if (counter == 3) {
						if (tiles [c, r] != null)
							tiles [c, r].tileObject.SetActive (false);
						if (tiles [c - 1, r] != null)
							tiles [c - 1, r].tileObject.SetActive (false);
						if (tiles [c - 2, r] != null)
							tiles [c - 2, r].tileObject.SetActive (false);
						tiles [c, r] = null;
						tiles [c - 1, r] = null;
						tiles [c - 2, r] = null;
						//renewBoard = true;
					}
				}
			}
		}
		//row check
		for (int c = 0; c < cols; c++){
			counter=1;
			for(int r=1;r<rows;r++){
				if (tiles [c, r] != null && tiles [c , r-1] != null) {
					if (tiles [c, r].type == tiles [c, r-1].type) {
						counter++;
					} else
						counter = 1;
					if (counter == 3) {
						if (tiles [c, r] != null) 
							tiles [c, r].tileObject.SetActive (false);
						if (tiles [c , r-1] != null)
							tiles [c , r-1].tileObject.SetActive (false);
						if (tiles [c , r-2] != null)
							tiles [c , r-2].tileObject.SetActive (false);
						tiles [c, r] = null;
						tiles [c, r-1] = null;
						tiles [c, r-2] = null;
						renewGrid = true;
					}
				}
			}
		}
		if (renewGrid) {
			RenewGrid ();
			renewGrid = false;
			Debug.Log ("True");
		}
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
