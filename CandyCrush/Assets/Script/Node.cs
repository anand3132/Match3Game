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

	public enum STATE {
		ACTIVE,
		DESTROYED,
		STATE_MAX
	}

	public static Color[] presetColors = {
		Color.blue
		, Color.cyan
		, Color.green
		, Color.magenta
		, Color.red
	};
	public TYPE nodeType;
	private STATE nodeState;

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

	public SpriteRenderer GetSpriteRenderer() {
		return GetComponent<SpriteRenderer> ();
	}

	public void SetDebugColor(Color clr) {
		SetColor (clr);
	}

	public void SetState(STATE state) {
		nodeState = state;

		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		switch (nodeState) {
		case STATE.ACTIVE: {
				if (renderer) {
					renderer.color = new Color(renderer.color.r,
						renderer.color.g,
						renderer.color.b,
						1.0f);
				}
				break;
			}
		case STATE.DESTROYED: {
				if (renderer) {
					renderer.color = new Color(renderer.color.r,
						renderer.color.g,
						renderer.color.b,
						0.0f);
				}
				break;
			}
		}
	}

	public STATE GetState() {
		return nodeState;
	}

//	public Color GetColor() {
//		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
//		if (renderer) {
//			return renderer.material.color;
//		}
//		return Color.white;
//	}
}
