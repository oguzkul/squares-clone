using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableSquare {

	public float rgbR;
	public float rgbG;
	public float rgbB;
	public float rgbA;
	public int x;
	public int y;
	public float scale = 1f;

	public SerializableSquare(){}

	public SerializableSquare(Square square){
		x = square.x;
		y = square.y;
		rgbR = square.color.r;
		rgbG = square.color.g;
		rgbB = square.color.b;
		rgbA = square.color.a;
		scale = square.transform.localScale.x;
	}

	public void setWithSquare(Square square){
		x = square.x;
		y = square.y;
		rgbR = square.color.r;
		rgbG = square.color.g;
		rgbB = square.color.b;
		rgbA = square.color.a;
		scale = square.transform.localScale.x;
	}

	public void setSquare(Square square){
		square.x = x;
		square.y = y;
		square.ChangeColor (new Color(rgbR, rgbG, rgbB, rgbA));
		square.transform.localScale = new Vector3(1f, 1f, 1f) * scale;
	}
}
