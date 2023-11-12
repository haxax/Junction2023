using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
	[SerializeField] private List<string> targetKeys = new List<string>();
	[SerializeField] private int checksPerFrame = 5;

	[Space(20)]
	[SerializeField] private float cooldown = 1.0f;
	[SerializeField] private float shootDistance = 3f;
	[SerializeField] private Poolable bulletPrefab;

	private float cooldownTimer = 0.0f;

	private int keyTracker = 0;
	private int instanceTracker = 0;

	private int remainingChecks = 0;

	private void FixedUpdate()
	{
		if (cooldownTimer > 0.0f) { cooldownTimer -= Time.fixedDeltaTime; }

		TargetScan();
	}

	private void TargetScan()
	{
		if (cooldownTimer > 0.0f) { return; }

		remainingChecks = checksPerFrame;

		for (; instanceTracker >= 0; instanceTracker--)
		{
			remainingChecks--;
			if (remainingChecks < 0) { break; }

			if (instanceTracker >= PoolManager.Instance.Instances.Count) { continue; }

			if (PoolManager.Instance.Instances[instanceTracker].PoolKey == targetKeys[keyTracker])
			{
				if (Vector2.Distance(PoolManager.Instance.Instances[instanceTracker].transform.position, transform.position) < shootDistance)
				{ Shoot(PoolManager.Instance.Instances[instanceTracker]); break; }
			}

			if (instanceTracker == 0)
			{
				keyTracker--;
				if (keyTracker < 0) { keyTracker = targetKeys.Count - 1; }
			}
		}
		if (instanceTracker < 0) { instanceTracker = PoolManager.Instance.Instances.Count - 1; }
	}

	private void Shoot(Poolable target)
	{
		cooldownTimer += cooldown;
		Projectile projectile = bulletPrefab.Spawn(transform.position).GetComponent<Projectile>();
		projectile.ShootAt(target.transform);
	}
}
