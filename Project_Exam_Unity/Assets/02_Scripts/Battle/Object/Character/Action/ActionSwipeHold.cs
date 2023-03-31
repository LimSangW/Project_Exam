using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionSwipeHold : SpecialActionBase
{
	private float SwipeSensitivity => ControlCharacterBase.SwipeSensitivity;
	private float holdTime;
	private float nowTime;
	private float beforeTime;
	private Vector2 beginPos;
	private Vector2 nowPos;
	private bool isPointerDown;

	public void Init(SpecialActionDelegate callback, float holdTime)
	{
		specialAction = callback;
		this.holdTime = holdTime;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		nowTime = 0;
		beforeTime = Time.time;
		beginPos = eventData.position;
		nowPos = beginPos;
		isPointerDown = true;
	}

	public void OnPointerUpdate(PointerEventData eventData)
	{
		nowTime += Time.time - beforeTime;
		beforeTime = Time.time;
		nowPos = eventData.position;
	}

	public void OnPointerUp()
	{
		nowTime = 0;
		isPointerDown = false;
	}

	public bool CheckSwipeHold()
	{
		bool isSwipe;
		bool isHold;

		isHold = nowTime > holdTime;

		var touchDif = nowPos - beginPos;
		float xDifAbs = Mathf.Abs(touchDif.x);
		float yDifABs = Mathf.Abs(touchDif.y);

		if (xDifAbs > SwipeSensitivity || yDifABs > SwipeSensitivity)
			isSwipe = true;
		else 
			isSwipe = false;

		return isSwipe && isHold && isPointerDown;
	}

	public override void PlaySpecialAction(DirectionType direction)
	{
		specialAction?.Invoke(direction);
	}
}