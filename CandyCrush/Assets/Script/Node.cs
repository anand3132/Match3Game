using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	public enum TYPE {
		NODE_0,
		NODE_1,
		NODE_2,
		NODE_3,
		NODE_4,
		NODE_MAX
	}

	public static Color[] presetColors = {
		Color.blue
		, Color.cyan
		, Color.green
		, Color.magenta
		, Color.red
	};
	public TYPE nodeType;
	public void SetNodeType(TYPE type) {
		nodeType = type;
		SetColor (presetColors [(int)nodeType]);
	}

	public TYPE GetNodeType() {
		return nodeType;
	}

	private void SetColor(Color clr) {
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		if (renderer) {
			renderer.material.color = clr;
		}
	}

	public void SetDebugColor(Color clr) {
		SetColor (clr);
	}

//	public Color GetColor() {
//		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
//		if (renderer) {
//			return renderer.material.color;
//		}
//		return Color.white;
//	}
}
