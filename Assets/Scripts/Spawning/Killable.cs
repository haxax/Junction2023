using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Killable : MonoBehaviour
{
	[SerializeField] private Poolable poolableReference;
	[SerializeField] private float maxHealth = 1f;
	[SerializeField] private UnityEvent OnHit;
	[SerializeField] private UnityEvent OnDeath;

	public float CurrentHealth { get; private set; }

	private void Awake()
	{
		poolableReference.OnSpawn.AddListener(OnSpawn);
		OnSpawn();
	}

	public void OnSpawn()
	{
		CurrentHealth = maxHealth;
	}

	public void DealDmg(float amount)
	{
		if (CurrentHealth <= 0f) { return; }

		if (amount > 0f) { OnHit?.Invoke(); }
		CurrentHealth -= amount;

		if (CurrentHealth <= 0f) { OnDeath?.Invoke(); }
	}
}
