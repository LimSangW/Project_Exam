using System;
using System.Collections.Generic;

public abstract class SpecialActionBase
{
	public delegate void SpecialActionDelegate(DirectionType direction);
	protected SpecialActionDelegate specialAction;

	public abstract void PlaySpecialAction(DirectionType direction);
}
