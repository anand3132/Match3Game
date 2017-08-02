using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	public void SetColor(Color clr) {
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		if (renderer) {
			renderer.material.color = clr;
		}
	}

	public Color GetColor() {
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		if (renderer) {
			return renderer.material.color;
		}
		return Color.white;
	}
}
