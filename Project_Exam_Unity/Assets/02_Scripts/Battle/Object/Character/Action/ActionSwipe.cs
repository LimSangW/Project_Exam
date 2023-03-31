using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionSwipe : SpecialActionBase
{
	private float SwipeSensitivity => ControlCharacterBase.SwipeSensitivity;
	private Vector2 beginPos;
	private Vector2 nowPos;

	public void Init(SpecialActionDelegate callback, float holdTime)
	{
		specialAction = callback;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		beginPos = eventData.position;
		nowPos = beginPos;
	}

	public void OnPointerUpdate(PointerEventData eventData)
	{
		nowPos = eventData.position;
	}

	public bool CheckSwipe()
	{
		bool isSwipe;

		var touchDif = nowPos - beginPos;
		float xDifAbs = Mathf.Abs(touchDif.x);
		float yDifABs = Mathf.Abs(touchDif.y);

		if (xDifAbs > SwipeSensitivity || yDifABs > SwipeSensitivity)
			isSwipe = true;
		else 
			isSwipe = false;

		return isSwipe;
	}

	public override void PlaySpecialAction(DirectionType direction)
	{
		specialAction?.Invoke(direction);
	}
}