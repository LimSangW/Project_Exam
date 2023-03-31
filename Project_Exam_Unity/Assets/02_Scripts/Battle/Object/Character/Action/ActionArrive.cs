using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionArrive : SpecialActionBase
{
	private Rect target;
	private bool activeTarget;

	public void Init(SpecialActionDelegate callback, Rect target)
	{
		specialAction= callback;
		SetTargetPosition(target);
	}

	public void SetTargetPosition(Rect target)
	{
		this.target = target;
		activeTarget = true;
	}

	public void OffTargetPosition()
	{
		activeTarget = false;
	}

	public bool CheckArriveTarget(Rect position)
	{
		return activeTarget && position.Overlaps(target);
	}

	public override void PlaySpecialAction(DirectionType direction)
	{
		specialAction?.Invoke(direction);
		OffTargetPosition();
	}
}
