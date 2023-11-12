using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
	public List<(string, Queue<Poolable>)> Poolables { get; private set; } = new List<(string, Queue<Poolable>)>();
	public List<Poolable> Instances { get; private set; } = new List<Poolable>();

	public Poolable GetFromPool(Poolable prefab)
	{ return GetFromPool(prefab, Vector2.zero); }

	public Poolable GetFromPool(Poolable prefab, Vector2 position)
	{
		Poolable result = null;

		bool found = false;
		foreach (var pool in Poolables)
		{
			if (pool.Item1 != prefab.PoolKey)
			{ continue; }

			if (pool.Item2.Count > 0)
			{ result = pool.Item2.Dequeue(); }

			found = true;
			break;
		}

		if (!found)
		{
			Poolables.Add((prefab.PoolKey, new Queue<Poolable>()));
		}

		if (result == null)
		{
			result = Instantiate(prefab);
			result.PoolManagerCreate();
		}
		result.transform.position = position;
		result.transform.rotation = Quaternion.identity;

		result.PoolManagerSpawn();
		result.gameObject.SetActive(true);

		Instances.Add(result);
		return result;
	}

	public void ReturnToPool(Poolable instance)
	{
		instance.PoolManagerDespawn();
		instance.gameObject.SetActive(false);
		Instances.Remove(instance);

		foreach (var pool in Poolables)
		{
			if (pool.Item1 != instance.PoolKey)
			{ continue; }

			pool.Item2.Enqueue(instance);
			return;
		}

		Poolables.Add((instance.PoolKey, new Queue<Poolable>()));
		Poolables[Poolables.Count - 1].Item2.Enqueue(instance);
	}
}