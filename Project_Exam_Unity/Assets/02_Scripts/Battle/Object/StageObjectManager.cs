using System;
using System.Collections.Generic;
using UnityEngine;

public class StageObjectManager : Manager<StageObjectManager>
{
	private List<ObjectBase> listObjects;
	public List<ObjectBase> ListObjects => listObjects;

	public override void Init()
	{
		listObjects = new List<ObjectBase>();
	}

	public override void ClearData()
	{
		listObjects.Clear();
	}

	public void SpawnObject(ObjectBase spawnObject)
	{
		// TODO: 해당 오브젝트를 필드에 생성하는 코드 추가 필요
		ListObjects.Add(spawnObject);
	}

	public void RemoveObject(ObjectBase removeObject)
	{
		if (ListObjects.Contains(removeObject) == false)
			throw new Exception($"{removeObject.name} is Not Contains in Manager's Object List");
		ListObjects.Remove(removeObject);
		removeObject.gameObject.SetActive(false);
	}

	public bool TryGetObjectInPoint(out ObjectBase target, Vector2 point)
	{
		foreach (ObjectBase obj in ListObjects)
		{
			if (obj.Pos.Contains(point))
			{
				target = obj;
				return true;
			}
		}
		target = null;
		return false;
	}

	public bool TryGetObjectInPoint<T>(out T target, Vector2 point) where T : ObjectBase
	{
		foreach (ObjectBase obj in ListObjects)
		{
			if (obj is T && obj.Pos.Contains(point))
			{
				target = obj as T;
				return true;
			}
		}
		target = null;
		return false;
	}

	public bool TryGetObjectsInRect(out List<ObjectBase> targets, Rect rect)
	{
		targets = new List<ObjectBase>();
		foreach (ObjectBase obj in ListObjects)
		{
			if (obj.Pos.Overlaps(rect))
			{
				targets.Add(obj);
			}
		}

		if (targets.Count == 0)
			return false;
		return true;
	}

	public bool TryGetObjectsInRect<T>(out List<T> targets, Rect rect) where T : ObjectBase
	{
		targets = new List<T>();
		foreach (ObjectBase obj in ListObjects)
		{
			if (obj is T && obj.Pos.Overlaps(rect))
			{
				targets.Add(obj as T);
			}
		}

		if (targets.Count == 0)
			return false;
		return true;
	}
}