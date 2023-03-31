using System;
using System.Collections.Generic;

public class ActionStack : SpecialActionBase
{
	private int stack;
	private int maxStack;

	public void Init(SpecialActionDelegate callback, int maxStack)
	{
		specialAction = callback;
		this.maxStack = maxStack;
	}

	public void AddStack()
	{
		stack++;
	}

	public bool CheckActionStack()
	{
		return stack >= maxStack;
	}

	public override void PlaySpecialAction(DirectionType direction)
	{
		specialAction?.Invoke(direction);
		stack = 0;
	}
}