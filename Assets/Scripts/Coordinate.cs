using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Coordinate {
	public int x;
	public int y;

	public Coordinate(){}

	public Coordinate(int _x, int _y){
		x = _x;
		y = _y;
	}

	public override bool Equals(object coordinate)
	{
		Coordinate item = coordinate as Coordinate;

		if (item == null)
		{
			return false;
		}

		return ( item.x == x & item.y == y);
	}
}
