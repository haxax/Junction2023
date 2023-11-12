using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OutOfBoundsDespawner : MonoBehaviour
{
	[SerializeField] private List<string> despawnKeys = new List<string>();
	[SerializeField] private int checksPerFrame = 5;

	private int keyTracker = 0;
	private int instanceTracker = 0;

	private int remainingChecks = 0;

	private void FixedUpdate()
	{
		remainingChecks = checksPerFrame;

		for (; instanceTracker >= 0; instanceTracker--)
		{
			remainingChecks--;
			if (remainingChecks < 0) { break; }

			if (instanceTracker >= PoolManager.Instance.Instances.Count) { continue; }

			if (PoolManager.Instance.Instances[instanceTracker].PoolKey == despawnKeys[keyTracker])
			{
				if (!PoolManager.Instance.Instances[instanceTracker].transform.position.IsWithinBounds())
				{ PoolManager.Instance.Instances[instanceTracker].Despawn(); }
			}

			if (instanceTracker == 0)
			{
				keyTracker--;
				if (keyTracker < 0) { keyTracker = despawnKeys.Count - 1; }
			}
		}
		if (instanceTracker < 0) { instanceTracker = PoolManager.Instance.Instances.Count - 1; }
	}
}
