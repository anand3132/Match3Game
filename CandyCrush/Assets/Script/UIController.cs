using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public GameObject playButton;
	public GameObject resetButton;
	public GameObject menueButton;
	public Text scoreBoard;
	public int score;
	NodeGenerator nodeGenerator;
	// Use this for initialization
	void Start () {
		score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		scoreBoard.text = score.ToString ();

	}

	public void SwitchToMainMenu() {

		playButton.SetActive (true);
		menueButton.SetActive (false);
		resetButton.SetActive (false);
		scoreBoard.text = "";
	}

	public void SwitchToIngameMenu() {
		playButton.SetActive (false);
		menueButton.SetActive (true);
		resetButton.SetActive (true);
		scoreBoard.text = score.ToString ();
	} 
	public void resetGame(){
	}
	public void AddScore() {
		score++;
	}
}
