using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ControlCharacterBase : CharacterBase, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	public const float SwipeSensitivity = 1.0f; // TODO: 1.0f는 임시값. 추후 적절한 값으로 변경 필요.
	private bool isDrag;
	private Vector2 beginTouchPoint;
	private Vector2 endTouchPoint;

	protected bool canAction;

	public override void ActionCommand(DirectionType direction)
	{
		if (canAction)
		{
			var atkRange = GetSearchRect(direction, atkModel.ActionRange);
			if (StageObjectManager.Instance.TryGetObjectsInRect<EnemyCharacterBase>(out var atkList, atkRange))
				ActionAttack(direction);
			else
				ActionMove(direction);

			canAction = false;
		}
	}

	public override void ActionMove(DirectionType direction)
	{
		//TODO: 1로 표기된 값은 임시값으로, 추후 변경 필요
		float moveTime = 1;
		float moveDis = 1 * moveModel.ActionRange;
		int moveRange = moveModel.ActionRange;

		// 이동 범위에 오브젝트가 있는 경우, 이동 거리 조정
		if (StageObjectManager.Instance.TryGetObjectsInRect(out var list, GetSearchRect(direction, moveModel.ActionRange)))
		{
			if (moveModel.ActionRange > 1)
			{
				int directionInt = (int)direction;
				foreach (var item in list)
				{
					if (item.ObjectType is ObjectType.Character or ObjectType.Obstacle)
					{
						int distance;
						if (dx[directionInt] != 0)
							distance = (int)Math.Abs(item.Pos.x - Pos.x);
						else
							distance = (int)Math.Abs(item.Pos.y - Pos.y);

						if (distance <= moveRange)
							moveRange = distance - 1;
					}
				}
			}
			else
				moveRange = 0;
		}

		// 좌표 변경 및 애니메이션 실행
		MovePos(direction, moveRange);
		animator.SetTrigger(MoveAnimKey);

		// 오브젝트 이동
		switch (direction)
		{
			case DirectionType.UP:
				transform.DOMoveY(transform.position.y + moveDis, moveTime).OnComplete(() => ActionDelay(moveModel.ActionCooldown));
				break;
			case DirectionType.DOWN:
				transform.DOMoveY(transform.position.y - moveDis, moveTime).OnComplete(() => ActionDelay(moveModel.ActionCooldown));
				break;
			case DirectionType.RIGHT:
				render.flipX = false;
				transform.DOMoveX(transform.position.x + moveDis, moveTime).OnComplete(() => ActionDelay(moveModel.ActionCooldown));
				break;
			case DirectionType.LEFT:
				render.flipX = true;
				transform.DOMoveX(transform.position.x - moveDis, moveTime).OnComplete(() => ActionDelay(moveModel.ActionCooldown));
				break;
		}
	}

	public override void ActionAttack(DirectionType direction)
	{
		animator.SetTrigger(AtkAnimKey);
		ActionDelay(atkModel.ActionCooldown);
		attackDirection = direction;
		switch (direction)
		{
			case DirectionType.RIGHT:
				render.flipX = false;
				break;
			case DirectionType.LEFT:
				render.flipX = true;
				break;
		}
	}

	public override void ActionHit(int damage)
	{
		damage -= model.Defense;
		if (damage <= 0) damage = 1;

		model.Hp -= damage;
		hpBar.fillAmount = model.GetHpAmount();
		if (model.Hp > 0)
			animator.SetTrigger(HitAnimKey);
		else
			ActionDead();
	}

	public override void ActionDead()
	{
		animator.SetBool(DeadAnimKey, true);
	}

	public override void HitDetection()
	{
		var atkRange = GetSearchRect(attackDirection, atkModel.ActionRange);
		if (StageObjectManager.Instance.TryGetObjectsInRect<EnemyCharacterBase>(out var list, atkRange))
		{
			foreach (var item in list)
			{
				item.ActionHit(model.Attack);
			}
		}
	}

	/// <summary>
	/// 행동이 실행된 후 다음 행동까지의 쿨타임을 실행시킴
	/// </summary>
	/// <param name="time">해당 행동의 고유 쿨타임을 받는다.</param>
	public Coroutine ActionDelay(float time)
	{
		float cooldown = time / model.Speed;
		return StartCoroutine(ActionDelayRoutine(cooldown));
	}

	private IEnumerator ActionDelayRoutine(float time)
	{
		cooldownUi.gameObject.SetActive(true);
		cooldownBar.fillAmount = 0;

		float t = 0;
		while (t < time)
		{
			t += Time.deltaTime;
			float fill = Mathf.InverseLerp(0, time, t);
			cooldownBar.fillAmount = fill;
			yield return new WaitForFixedUpdate();
		}

		canAction = true;
		cooldownUi.gameObject.SetActive(false);
		yield return null;
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		isDrag = false;
		beginTouchPoint = eventData.position;
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		isDrag = true;
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (isDrag)
		{
			endTouchPoint = eventData.position;
			var touchDif = endTouchPoint- beginTouchPoint;

			float xDifAbs = Mathf.Abs(touchDif.x);
			float yDifABs = Mathf.Abs(touchDif.y);

			if (xDifAbs > SwipeSensitivity || yDifABs > SwipeSensitivity)
			{
				DirectionType swipeDirection;
				if (xDifAbs > yDifABs)
				{
					swipeDirection = touchDif.x > 0 ? DirectionType.RIGHT : DirectionType.LEFT;
				}
				else
				{
					swipeDirection = touchDif.y > 0 ? DirectionType.UP : DirectionType.DOWN;
				}

				ActionCommand(swipeDirection);
			}
		}
	}
}