using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Poolable : MonoBehaviour
{
	[SerializeField] private string poolKey;
	[SerializeField] private UnityEvent onCreate;
	[SerializeField] private UnityEvent onSpawn;
	[SerializeField] private UnityEvent onDespawn;

	public string PoolKey => poolKey;
	public UnityEvent OnCreate => onCreate;
	public UnityEvent OnSpawn => onSpawn;
	public UnityEvent OnDespawn => onDespawn;

	public void PoolManagerCreate() { OnCreate?.Invoke(); }
	public void PoolManagerSpawn() { OnSpawn?.Invoke(); }
	public void PoolManagerDespawn() { OnDespawn?.Invoke(); }

	public Poolable Spawn() { return Spawn(Vector2.zero); }
	public Poolable Spawn(Vector2 position) { return PoolManager.Instance.GetFromPool(this, position); }

	[ContextMenu("Despawn")]
	public void Despawn() { PoolManager.Instance.ReturnToPool(this); }
}