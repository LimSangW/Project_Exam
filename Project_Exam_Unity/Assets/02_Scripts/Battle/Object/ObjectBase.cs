using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
	protected static readonly int[] dx = {-1, 1, 0, 0};
	protected static readonly int[] dy = {0, 0, 1, -1};

	private Rect pos;
	public Rect Pos => pos;

	public ObjectType ObjectType;

	public void MovePos(DirectionType direction, int Range)
	{
		int directionInt = (int)direction;

		Rect newPos = Pos;
		newPos.x += dx[directionInt] * Range;
		newPos.y += dy[directionInt] * Range;
		pos = newPos;
	}
}
