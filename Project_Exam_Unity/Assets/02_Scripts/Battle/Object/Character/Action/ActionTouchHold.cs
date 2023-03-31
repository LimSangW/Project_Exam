using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ActionTouchHold : SpecialActionBase
{
	private float holdTime;
	private float nowTime;
	private float beforeTime;
	private bool isPointerDown;

	public void Init(SpecialActionDelegate callback, float holdTime)
	{
		specialAction = callback;
		this.holdTime = holdTime;
	}

	public void OnPointerDown()
	{
		nowTime = 0;
		beforeTime = Time.time;
		isPointerDown = true;
	}

	public void OnPointerUpdate()
	{
		nowTime += Time.time - beforeTime;
		beforeTime = Time.time;
	}

	public void OnPointerUp()
	{
		nowTime = 0;
		isPointerDown = false;
	}

	public bool CheckHold()
	{
		return isPointerDown && nowTime > holdTime;
	}

	public override void PlaySpecialAction(DirectionType direction)
	{
		specialAction?.Invoke(direction);
	}
}