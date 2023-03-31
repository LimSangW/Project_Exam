using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class CharacterBase : ObjectBase
{
	protected CharacterStatModel model;
	protected CharacterActionModel moveModel;
	protected CharacterActionModel atkModel;

	[SerializeField] protected SpriteRenderer render;
	[SerializeField] protected Image hpBar;
	[SerializeField] protected GameObject cooldownUi;
	[SerializeField] protected Image cooldownBar;

	[SerializeField] protected Animator animator;
	protected const string MoveAnimKey = "MoveKey";
	protected const string AtkAnimKey = "AttackKey";
	protected const string HitAnimKey = "HitKey";
	protected const string DeadAnimKey = "DeadKey";

	protected DirectionType attackDirection;

	/// Dummy
	public void Init()
	{
		int maxhp = 10;
		int attack = 3;
		int defense = 1;
		float speed = 3;

		ObjectType = ObjectType.Character;

		model = new CharacterStatModel(maxhp, attack, defense, speed);
		moveModel = new CharacterActionModel(1, 3);
		atkModel = new CharacterActionModel(1, 3);
	}

	public void Release()
	{
		StageObjectManager.Instance.RemoveObject(this);
	}

	public Rect GetSearchRect(DirectionType direction, int Range)
	{
		int directionInt = (int)direction;

		Rect searchRect = Pos;
		searchRect.x += dx[directionInt] * Range;
		searchRect.y += dy[directionInt] * Range;
		searchRect.width = dx[directionInt] == 0 ? Pos.width : Range;
		searchRect.height = dy[directionInt] == 0 ? Pos.height : Range;
		return searchRect;
	}

	public abstract void ActionCommand(DirectionType direction);
	public abstract void ActionMove(DirectionType direction);
	public abstract void ActionAttack(DirectionType direction);
	public abstract void ActionSpecial(DirectionType direction);
	public abstract void ActionHit(int damage);
	public abstract void ActionDead();

	public abstract void HitDetection();
}
